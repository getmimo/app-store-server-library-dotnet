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

    [Fact]
    public async Task VerifyAndDecode_WithBundleIdOverride_Success()
    {
        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_TestNotification_Success.txt"
        );

        // Construct verifier with a wrong default bundleId
        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            "com.wrong.default"
        );

        // Override with the correct bundleId — should succeed
        ResponseBodyV2DecodedPayload result = await dataVerifier.VerifyAndDecodeNotification(
            testNotificationPayload,
            bundleId: BundleId
        );

        Assert.NotNull(result);
        Assert.Equal("TEST", result.NotificationType);
    }

    [Fact]
    public async Task VerifyAndDecode_WithWrongBundleIdOverride_Fails()
    {
        string testNotificationPayload = await File.ReadAllTextAsync(
            "./MockedSignedData/InputFor_VerifyAndDecode_TestNotification_Success.txt"
        );

        // Construct verifier with the correct default bundleId
        var dataVerifier = new SignedDataVerifier(
            Convert.FromBase64String(RootCaBase64Encoded),
            false,
            AppStoreEnvironment.Sandbox,
            BundleId
        );

        // Override with a wrong bundleId — override takes precedence, so it should fail
        var exception = await Assert.ThrowsAsync<VerificationException>(
            () => dataVerifier.VerifyAndDecodeNotification(testNotificationPayload, bundleId: "com.wrong.override")
        );

        Assert.Contains("BundleId in payload does not match expected bundleId.", exception.Message);
    }
}
