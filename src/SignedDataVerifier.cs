using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Mimo.AppStoreServerLibrary.Exceptions;
using Mimo.AppStoreServerLibrary.Models;

namespace Mimo.AppStoreServerLibrary;

public class SignedDataVerifier(
    byte[][] appleRootCertificates,
    bool enableOnlineChecks,
    AppStoreEnvironment environment,
    string bundleId
)
{
    // To compatibility with the previous version (<= 0.1.0) of the constructor
    public SignedDataVerifier(
        byte[] appleRootCertificate,
        bool enableOnlineChecks,
        AppStoreEnvironment environment,
        string bundleId
    )
        : this([appleRootCertificate], enableOnlineChecks, environment, bundleId) { }

    /// <summary>
    /// Verifies and decodes an App Store Server Notification signedPayload.
    /// See <see href="https://developer.apple.com/documentation/appstoreservernotifications/signedpayload">signedPayload</see>
    /// </summary>
    /// <param name="signedPayload">The payload received by your server</param>
    /// <returns>The decoded payload after verification</returns>
    /// <exception cref="VerificationException">Thrown if the data could not be verified</exception>
    public async Task<ResponseBodyV2DecodedPayload> VerifyAndDecodeNotification(string signedPayload)
    {
        string payload = await VerifySignedData(signedPayload);

        ResponseBodyV2DecodedPayload decodedPayload;

        try
        {
            decodedPayload = JsonSerializer.Deserialize(
                payload,
                AppStoreJsonContext.Default.ResponseBodyV2DecodedPayload
            )!;
        }
        catch (Exception e)
        {
            throw new VerificationException($"Error deserializing notification payload. Payload : {payload}", e);
        }

        (string payloadEnvironment, string payloadBundleId) = decodedPayload switch
        {
            { Data: { } data } => (data.Environment, data.BundleId),
            { Summary: { } summary } => (summary.Environment, summary.BundleId),
            _ => throw new VerificationException("Notification payload has neither data nor summary"),
        };

        ValidateEnvironment(payloadEnvironment);
        ValidateBundleId(payloadBundleId);

        return decodedPayload;
    }

    /// <summary>
    /// Verifies and decodes a signedTransaction obtained from the App Store Server API, an App Store Server Notification, or from a device.
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/jwstransaction">JWSTransaction</see>
    /// </summary>
    /// <param name="signedPayload">The signedTransaction field</param>
    /// <returns>The decoded transaction info after verification</returns>
    /// <exception cref="VerificationException">Thrown if the data could not be verified</exception>
    public async Task<JwsTransactionDecodedPayload> VerifyAndDecodeTransaction(string signedPayload)
    {
        string payload = await VerifySignedData(signedPayload);

        JwsTransactionDecodedPayload transaction;
        try
        {
            transaction = JsonSerializer.Deserialize(
                payload,
                AppStoreJsonContext.Default.JwsTransactionDecodedPayload
            )!;
        }
        catch (Exception e)
        {
            throw new VerificationException($"Error deserializing transaction payload. Payload : {payload}", e);
        }

        ValidateEnvironment(transaction.Environment);
        ValidateBundleId(transaction.BundleId);

        return transaction;
    }

    /// <summary>
    /// Verifies and decodes a signedRenewalInfo obtained from the App Store Server API, an App Store Server Notification, or from a device.
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/jwsrenewalinfo">JWSRenewalInfo</see>
    /// </summary>
    /// <param name="signedPayload">The signedRenewalInfo field</param>
    /// <returns>The decoded renewal info after verification</returns>
    /// <exception cref="VerificationException">Thrown if the data could not be verified</exception>
    public async Task<JWSRenewalInfoDecodedPayload> VerifyAndDecodeRenewalInfo(string signedPayload)
    {
        string payload = await VerifySignedData(signedPayload);

        JWSRenewalInfoDecodedPayload renewalInfo;
        try
        {
            renewalInfo = JsonSerializer.Deserialize(
                payload,
                AppStoreJsonContext.Default.JWSRenewalInfoDecodedPayload
            )!;
        }
        catch (Exception e)
        {
            throw new VerificationException($"Error deserializing renewal info payload. Payload : {payload}", e);
        }

        ValidateEnvironment(renewalInfo.Environment);

        return renewalInfo;
    }

    private async Task<string> VerifySignedData(string signedPayload)
    {
        //1. Verify the payload is composed of 3 parts separated by a dot => Indicates the JWS is well formed
        //2. Decode the payload to verify that there is a header, a payload and a signature => Indicates the JWS is well formed
        //3. Parse the header and verify it contains an x5c claim with 3 certificates AND that the alg claim == ES256
        //4. Verify signature chain and check for revocation
        //5. Get the public key from the leaf certificate (1st certificate in x5c claim) and verify the payloads signature with it.

        // Split the payload into 3 parts
        string[] parts = signedPayload.Split('.');
        if (parts.Length != 3)
        {
            throw new VerificationException("Payload does not have the correct format");
        }

        // Decode the header and the payload , which are the 1st and 2nd parts of the payload
        // We do not use a JsonWebToken to parse the token because it does not expose the x5c claim that we need to verify the signature
        string payloadJson;
        JWSDecodedHeader? header;
        try
        {
            string headerJson = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(parts[0]));
            payloadJson = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(parts[1]));
            header = JsonSerializer.Deserialize(headerJson, AppStoreJsonContext.Default.JWSDecodedHeader);
        }
        catch (Exception e)
        {
            throw new VerificationException("Payload header or body could not be decoded", e);
        }

        //Check if Environment is local testing, in this case data may not be signed by the App Store, and verification should be skipped
        if (environment == AppStoreEnvironment.LocalTesting)
        {
            return payloadJson;
        }

        if (header?.x5c == null || header.x5c.Length != 3)
        {
            throw new VerificationException("x5c claim is null or has more or less than 3 certificates");
        }

        if (header.Alg != "ES256")
        {
            throw new VerificationException($"Unrecognized JWT algorithm attribute : {header.Alg}");
        }

        //We don't include the root cert in the path, we use the one downloaded from https://www.apple.com/certificateauthority/AppleRootCA-G3.cer
        //See Apple implementation for Java : https://github.com/apple/app-store-server-library-java/blob/main/src/main/java/com/apple/itunes/storekit/verification/ChainVerifier.java#L70C14-L71C90
        //or Node : https://github.com/apple/app-store-server-library-node/blob/main/jws_verification.ts#L185
        X509Certificate2Collection col = new();
        try
        {
            foreach (string c in header.x5c[..2])
            {
                byte[] bytes = Convert.FromBase64String(c);
                col.Add(new X509Certificate2(bytes));
            }
        }
        catch (Exception e)
        {
            throw new VerificationException("x5c certificates could not be decoded", e);
        }

        //As the header contains a x5c parameter we need to check for the certificate chain
        // As Apple's libraries do, validate offline at the time the payload was signed
        DateTime verificationTime = enableOnlineChecks ? DateTime.Now : ReadSignedDate(payloadJson) ?? DateTime.Now;

        bool certChainIsValid = this.CheckCertificateChain(col, verificationTime);

        if (!certChainIsValid)
        {
            throw new VerificationException("Certificate chain is invalid");
        }

        //We now need to verify the signature using the leaf (first) certificate of the x5c parameter.
        //This is done by retrieving it's public key and calling the JWT library to verify the signature
        var leafCert = col[0];

        var securityTokenHandler = new JsonWebTokenHandler();
        var leafPublicKey = new ECDsaSecurityKey(leafCert.GetECDsaPublicKey());
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = leafPublicKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            //Notifications don't have an expiration date
            ValidateLifetime = false,
        };

        TokenValidationResult? result = await securityTokenHandler.ValidateTokenAsync(
            signedPayload,
            validationParameters
        );

        if (!result.IsValid)
        {
            throw new VerificationException(
                $"Payload signature could not be verified : {result.Exception.Message}",
                result.Exception
            );
        }

        return payloadJson;
    }

    /// <summary>
    /// Checks the certificate chain of the notification
    /// Rfc for the x5c parameter : https://datatracker.ietf.org/doc/html/rfc7515#section-4.1.6
    /// Also see following discussion on Apple forum for better explanations : https://forums.developer.apple.com/forums/thread/693351
    /// </summary>
    /// <returns>If the certificate chain is valid</returns>
    private bool CheckCertificateChain(X509Certificate2Collection certificates, DateTime verificationTime)
    {
        var chain = new X509Chain();

        // Configure the chain to check for certificate revocation online.
        // Online check can result in a longer delay while the certificate authority is contacted.
        // It's still unclear if OCSP is supported by Apple, see following comment in the Java Library code base :
        // https://github.com/apple/app-store-server-library-java/blob/main/src/main/java/com/apple/itunes/storekit/verification/ChainVerifier.java#L70C14-L71C90
        // Also an explanation of the use of OCSP can be found here : https://forums.developer.apple.com/forums/thread/693351
        // The default value for DisableOnlineCertificateRevocationCheck is false
        chain.ChainPolicy.RevocationMode = enableOnlineChecks ? X509RevocationMode.Online : X509RevocationMode.NoCheck;
        chain.ChainPolicy.VerificationTime = verificationTime;

        // We need to set the trust mode to custom root trust so we can add our own root certificate
        // This is needed because the root certificate is not in the default trust store
        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

        // Add the root certificates to the trust store
        foreach (var rootCert in appleRootCertificates)
        {
            chain.ChainPolicy.CustomTrustStore.Add(new X509Certificate2(rootCert));
        }

        //Add the intermediate certificates to the extra store so they can be used to build the chain
        chain.ChainPolicy.ExtraStore.Add(certificates[1]);

        bool isValid = chain.Build(certificates[0]);

        if (!isValid)
        {
            var message =
                "Chain validation failed. "
                + string.Join(
                    " -> ",
                    chain.ChainStatus.Select(chainStatus =>
                        chainStatus.StatusInformation + " - Status : " + chainStatus.Status
                    )
                );
            throw new VerificationException(message);
        }

        // Marker extensions identifying App Store signing certificates, as checked by Apple's libraries
        const string leafOid = "1.2.840.113635.100.6.11.1";
        const string intermediateOid = "1.2.840.113635.100.6.2.1";

        if (chain.ChainElements.Count != 3)
        {
            throw new VerificationException($"Certificate chain has {chain.ChainElements.Count} elements, expected 3");
        }

        if (chain.ChainElements[0].Certificate.Extensions[leafOid] == null)
        {
            throw new VerificationException($"OID {leafOid} was not found on the signing certificate");
        }

        if (chain.ChainElements[1].Certificate.Extensions[intermediateOid] == null)
        {
            throw new VerificationException($"OID {intermediateOid} was not found on the intermediate certificate");
        }

        return isValid;
    }

    private static DateTime? ReadSignedDate(string payloadJson)
    {
        long milliseconds;
        try
        {
            using JsonDocument document = JsonDocument.Parse(payloadJson);
            if (
                document.RootElement.ValueKind != JsonValueKind.Object
                || !document.RootElement.TryGetProperty("signedDate", out JsonElement signedDate)
                || !signedDate.TryGetInt64(out milliseconds)
            )
            {
                return null;
            }
        }
        catch (JsonException e)
        {
            throw new VerificationException("Payload could not be decoded", e);
        }

        if (milliseconds < 0 || milliseconds > DateTimeOffset.MaxValue.ToUnixTimeMilliseconds())
        {
            throw new VerificationException($"signedDate {milliseconds} is out of range");
        }

        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
    }

    private void ValidateEnvironment(string? payloadEnvironment)
    {
        if (payloadEnvironment != environment.Name)
        {
            throw new VerificationException(
                $"Environment in payload does not match expected environment. Expected : {environment}, Actual : {payloadEnvironment}"
            );
        }
    }

    private void ValidateBundleId(string? payloadBundleId)
    {
        if (payloadBundleId != bundleId)
        {
            throw new VerificationException(
                $"BundleId in payload does not match expected bundleId. Expected : {bundleId}, Actual : {payloadBundleId}"
            );
        }
    }
}
