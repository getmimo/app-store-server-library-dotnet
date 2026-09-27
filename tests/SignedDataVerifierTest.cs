using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Mimo.AppStoreServerLibrary;
using Mimo.AppStoreServerLibrary.Exceptions;
using Mimo.AppStoreServerLibrary.Models;
using Xunit;

namespace Mimo.AppStoreServerLibraryTests;

/// <summary>
/// In this test class the input Data was generated in two ways :
/// - By manually requesting a test notification from the App Store server. See : https://developer.apple.com/documentation/appstoreserverapi/request_a_test_notification
/// - By retrieving the payload from the existing libraries maintained by Apple.
/// This way we are sure the payload is valid and signed by Apple.
/// </summary>
public class SignedDataVerifierTest
{
    private const string RootCaBase64Encoded =
        "MIIBgjCCASmgAwIBAgIJALUc5ALiH5pbMAoGCCqGSM49BAMDMDYxCzAJBgNVBAYTAlVTMRMwEQYDVQQIDApDYWxpZm9ybmlhMRIwEAYDVQQHDAlDdXBlcnRpbm8wHhcNMjMwMTA1MjEzMDIyWhcNMzMwMTAyMjEzMDIyWjA2MQswCQYDVQQGEwJVUzETMBEGA1UECAwKQ2FsaWZvcm5pYTESMBAGA1UEBwwJQ3VwZXJ0aW5vMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEc+/Bl+gospo6tf9Z7io5tdKdrlN1YdVnqEhEDXDShzdAJPQijamXIMHf8xWWTa1zgoYTxOKpbuJtDplz1XriTaMgMB4wDAYDVR0TBAUwAwEB/zAOBgNVHQ8BAf8EBAMCAQYwCgYIKoZIzj0EAwMDRwAwRAIgemWQXnMAdTad2JDJWng9U4uBBL5mA7WI05H7oH7c6iQCIHiRqMjNfzUAyiu9h6rOU/K+iTR0I/3Y/NSWsXHX+acc";

    private const string BundleId = "com.example";

    // Certificates from Apple's app-store-server-library-java ChainVerifierTest, issued by the test root above.
    private const string IntermediateCa =
        "MIIBnzCCAUWgAwIBAgIBCzAKBggqhkjOPQQDAzA2MQswCQYDVQQGEwJVUzETMBEGA1UECAwKQ2FsaWZvcm5pYTESMBAGA1UEBwwJQ3VwZXJ0aW5vMB4XDTIzMDEwNTIxMzEwNVoXDTMzMDEwMTIxMzEwNVowRTELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAkNBMRIwEAYDVQQHDAlDdXBlcnRpbm8xFTATBgNVBAoMDEludGVybWVkaWF0ZTBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABBUN5V9rKjfRiMAIojEA0Av5Mp0oF+O0cL4gzrTF178inUHugj7Et46NrkQ7hKgMVnjogq45Q1rMs+cMHVNILWqjNTAzMA8GA1UdEwQIMAYBAf8CAQAwDgYDVR0PAQH/BAQDAgEGMBAGCiqGSIb3Y2QGAgEEAgUAMAoGCCqGSM49BAMDA0gAMEUCIQCmsIKYs41ullssHX4rVveUT0Z7Is5/hLK1lFPTtun3hAIgc2+2RG5+gNcFVcs+XJeEl4GZ+ojl3ROOmll+ye7dynQ=";
    private const string LeafCertInvalidOid =
        "MIIBoDCCAUagAwIBAgIBDzAKBggqhkjOPQQDAzBFMQswCQYDVQQGEwJVUzELMAkGA1UECAwCQ0ExEjAQBgNVBAcMCUN1cGVydGlubzEVMBMGA1UECgwMSW50ZXJtZWRpYXRlMB4XDTIzMDEwNTIxMzczMVoXDTMzMDEwMTIxMzczMVowPTELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAkNBMRIwEAYDVQQHDAlDdXBlcnRpbm8xDTALBgNVBAoMBExlYWYwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAATitYHEaYVuc8g9AjTOwErMvGyPykPa+puvTI8hJTHZZDLGas2qX1+ErxgQTJgVXv76nmLhhRJH+j25AiAI8iGsoy8wLTAJBgNVHRMEAjAAMA4GA1UdDwEB/wQEAwIHgDAQBgoqhkiG92NkBgsCBAIFADAKBggqhkjOPQQDAwNIADBFAiAb+7S3i//bSGy7skJY9+D4VgcQLKFeYfIMSrUCmdrFqwIhAIMVwzD1RrxPRtJyiOCXLyibIvwcY+VS73HYfk0O9lgz";
    private const string IntermediateCaInvalidOid =
        "MIIBnjCCAUWgAwIBAgIBDTAKBggqhkjOPQQDAzA2MQswCQYDVQQGEwJVUzETMBEGA1UECAwKQ2FsaWZvcm5pYTESMBAGA1UEBwwJQ3VwZXJ0aW5vMB4XDTIzMDEwNTIxMzYxNFoXDTMzMDEwMTIxMzYxNFowRTELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAkNBMRIwEAYDVQQHDAlDdXBlcnRpbm8xFTATBgNVBAoMDEludGVybWVkaWF0ZTBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABBUN5V9rKjfRiMAIojEA0Av5Mp0oF+O0cL4gzrTF178inUHugj7Et46NrkQ7hKgMVnjogq45Q1rMs+cMHVNILWqjNTAzMA8GA1UdEwQIMAYBAf8CAQAwDgYDVR0PAQH/BAQDAgEGMBAGCiqGSIb3Y2QGAgIEAgUAMAoGCCqGSM49BAMDA0cAMEQCIFROtTE+RQpKxNXETFsf7Mc0h+5IAsxxo/X6oCC/c33qAiAmC5rn5yCOOEjTY4R1H1QcQVh+eUwCl13NbQxWCuwxxA==";
    private const string LeafCertForIntermediateCaInvalidOid =
        "MIIBnzCCAUagAwIBAgIBDjAKBggqhkjOPQQDAzBFMQswCQYDVQQGEwJVUzELMAkGA1UECAwCQ0ExEjAQBgNVBAcMCUN1cGVydGlubzEVMBMGA1UECgwMSW50ZXJtZWRpYXRlMB4XDTIzMDEwNTIxMzY1OFoXDTMzMDEwMTIxMzY1OFowPTELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAkNBMRIwEAYDVQQHDAlDdXBlcnRpbm8xDTALBgNVBAoMBExlYWYwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAATitYHEaYVuc8g9AjTOwErMvGyPykPa+puvTI8hJTHZZDLGas2qX1+ErxgQTJgVXv76nmLhhRJH+j25AiAI8iGsoy8wLTAJBgNVHRMEAjAAMA4GA1UdDwEB/wQEAwIHgDAQBgoqhkiG92NkBgsBBAIFADAKBggqhkjOPQQDAwNHADBEAiAUAs+gzYOsEXDwQquvHYbcVymyNqDtGw9BnUFp2YLuuAIgXxQ3Ie9YU0cMqkeaFd+lyo0asv9eyzk6stwjeIeOtTU=";

    [Fact]
    public async Task VerifyAndDecode_TestNotification_Success()
    {
        /*
          The test token contains the following header :
          {
             "alg": "ES256",
             "x5c": [
               "[Leaf-Certificate]",
               "[Intermediate-Certificate]",
               "[Root-Certificate]"
             ]
           }

           And payload :
           {
             "data": {
               "appAppleId": 1234,
               "environment": "Sandbox",
               "bundleId": "com.example"
             },
             "notificationUUID": "9ad56bd2-0bc6-42e0-af24-fd996d87a1e6",
             "signedDate": 1681314324000,
             "notificationType": "TEST"
           }
         */

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_TestNotification_Success.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );
        ResponseBodyV2DecodedPayload result = await dataVerifier.VerifyAndDecodeNotification(testNotificationPayload);

        Assert.IsType<ResponseBodyV2DecodedPayload>(result);
        Assert.NotNull(result);
        Assert.Equal("TEST", result.NotificationType);
        Assert.NotEqual(result.NotificationUuid, Guid.Empty);
    }

    [Fact]
    public async Task VerifyAndDecode_AlgParameterIsUnsupported_Fails()
    {
        //JWS was updated to set Alg parameter to HS256 - HMAC using SHA-256
        //Should return an error as it's not a supported algorithm

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_AlgParameterIsUnsupported_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Equal("Unrecognized JWT algorithm attribute : HS256", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_JWSIsMissingAPart_Fails()
    {
        //JWS was updated to remove the header
        //Should Fail as it's missing the first part

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_JWSIsMissingAPart_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Equal("Payload does not have the correct format", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_Nox5cParameter_Fails()
    {
        //JWS was updated to remove the chain certificate parameter (x5c)
        //Should failas it's required to verify the payload

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_Nox5cParameter_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Equal("x5c claim is null or has more or less than 3 certificates", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_ChainCertificateCompromised_Fails()
    {
        //JWS was updated to alter the chain certificate parameter (x5c), the first certificate was permuted with the second one.
        //Should fail as it should fail to verify the signature by using the wrongly set first certificate of the x5c parameter

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_ChainCertificateCompromised_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Contains("Chain validation failed", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_InvalidSignature_Fails()
    {
        //Manually update an already signed payload to have an invalid signature

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_InvalidSignature_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Contains("Payload signature could not be verified", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_RenewalInfo_Success()
    {
        /*
         * Decoded Renewal info is
         * {
             "environment": "Sandbox",
             "bundleId": "com.example",
             "signedDate": 1672956154000
           }
         */
        string didRenewNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_RenewalInfo_Success.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        JWSRenewalInfoDecodedPayload result = await dataVerifier.VerifyAndDecodeRenewalInfo(
            didRenewNotificationPayload
        );

        Assert.Equal("Sandbox", result.Environment);
    }

    [Fact]
    public async Task VerifyAndDecode_TransactionInfo_Success()
    {
        /*
         * Decoded Renewal info is
         * {
             "environment": "Sandbox",
             "bundleId": "com.example",
             "signedDate": 1672956154000
           }
         */
        string didRenewNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_TransactionInfo_Success.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        JwsTransactionDecodedPayload result = await dataVerifier.VerifyAndDecodeTransaction(
            didRenewNotificationPayload
        );

        Assert.Equal("Sandbox", result.Environment);
    }

    [Fact]
    public async Task VerifyAndDecode_WrongBundleId_Fails()
    {
        string wrongBundleId = "com.example.wrong";

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_WrongBundleId_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            wrongBundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Contains("BundleId in payload does not match expected bundleId.", exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_WrongEnvironment_Fails()
    {
        AppStoreEnvironment wrongEnvironment = AppStoreEnvironment.Production;

        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_WrongEnvironment_Fails.txt"
        );

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            wrongEnvironment,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload)
        );

        Assert.Contains("Environment in payload does not match expected environment.", exception.Message);
    }

    [Theory]
    [InlineData(LeafCertInvalidOid, IntermediateCa, "1.2.840.113635.100.6.11.1")]
    [InlineData(LeafCertForIntermediateCaInvalidOid, IntermediateCaInvalidOid, "1.2.840.113635.100.6.2.1")]
    public async Task VerifyAndDecode_CertificateMissingAppleOid_Fails(string leaf, string intermediate, string oid)
    {
        string header = JsonSerializer.Serialize(
            new { alg = "ES256", x5c = new[] { leaf, intermediate, RootCaBase64Encoded } }
        );
        string signedPayload = Base64UrlEncoder.Encode(header) + ".e30.AA";

        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(signedPayload)
        );

        Assert.Contains(oid, exception.Message);
    }

    [Fact]
    public async Task VerifyAndDecode_LeafIssuedByRoot_Fails()
    {
        using var rootKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var rootRequest = new CertificateRequest("CN=Root", rootKey, HashAlgorithmName.SHA256);
        rootRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        using X509Certificate2 root = rootRequest.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(1)
        );

        using var leafKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var leafRequest = new CertificateRequest("CN=Leaf", leafKey, HashAlgorithmName.SHA256);
        leafRequest.CertificateExtensions.Add(
            new X509Extension("1.2.840.113635.100.6.11.1", new byte[] { 0x05, 0x00 }, false)
        );
        using X509Certificate2 leaf = leafRequest.Create(
            root,
            DateTimeOffset.UtcNow.AddHours(-1),
            DateTimeOffset.UtcNow.AddHours(1),
            new byte[] { 1 }
        );

        string rootBase64 = Convert.ToBase64String(root.RawData);
        string header = JsonSerializer.Serialize(
            new { alg = "ES256", x5c = new[] { Convert.ToBase64String(leaf.RawData), rootBase64, rootBase64 } }
        );
        string signedPayload = Base64UrlEncoder.Encode(header) + ".e30.AA";

        var dataVerifier = new SignedDataVerifier(root.RawData, false, AppStoreEnvironment.Sandbox, BundleId);

        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(signedPayload)
        );

        Assert.Contains("Certificate chain has 2 elements", exception.Message);
    }
}
