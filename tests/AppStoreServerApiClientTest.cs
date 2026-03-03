using Mimo.AppStoreServerLibrary;
using Mimo.AppStoreServerLibrary.Models;
using RichardSzalay.MockHttp;
using Xunit;

namespace Mimo.AppStoreServerLibraryTests;

public class AppStoreServerApiClientTest
{
    private const string TestSigningKey = """
        -----BEGIN PRIVATE KEY-----
        MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgSpP55ELdXswj9JRZ
        APRwtTfS4CNRqpKIs+28rNHiPAqhRANCAASs8nLES7b+goKslppNVOurf0MonZdw
        3pb6TxS8Z/5j+UNY1sWK1ChxpuwNS9I3R50cfdQo/lA9PPhw6XIg8ytd
        -----END PRIVATE KEY-----
        """;
    private const string KeyId = "TEST123456";
    private const string IssuerId = "99b16628-15e4-4668-972c-d7934e8838b6";
    private const string BundleId = "com.example.app";

    private static AppStoreServerApiClient GetAppStoreServerApiClient(MockHttpMessageHandler mockHttp)
    {
        return new AppStoreServerApiClient(
            TestSigningKey,
            KeyId,
            IssuerId,
            BundleId,
            AppStoreEnvironment.LocalTesting,
            mockHttp.ToHttpClient()
        );
    }

    [Fact]
    public async Task GetAllSubscriptionStatuses_Success()
    {
        const string responseData = """
            {
              "environment" : "Sandbox",
              "bundleId" : "com.test.app",
              "appAppleId" : 1234567890,
              "data" : [ {
                "subscriptionGroupIdentifier" : "98765",
                "lastTransactions" : [ {
                  "originalTransactionId" : "123454321",
                  "status" : 2,
                  "signedTransactionInfo" : "eyabc",
                  "signedRenewalInfo" : "eyxyz"
                } ]
              } ]
            }
            """;
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When("https://local-testing-base-url/inApps/v1/subscriptions/123456")
            .Respond("application/json", responseData);

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);
        SubscriptionStatusResponse response = await client.GetAllSubscriptionStatuses("123456");

        Assert.NotNull(response);
        Assert.Equal("Sandbox", response.Environment);
        Assert.Equal("com.test.app", response.BundleId);
        Assert.Equal(1234567890, response.AppAppleId);
        Assert.Collection(
            response.Data,
            data =>
            {
                Assert.Equal("98765", data.SubscriptionGroupIdentifier);
                Assert.Collection(
                    data.LastTransactions,
                    transaction =>
                    {
                        Assert.Equal("123454321", transaction.OriginalTransactionId);
                        Assert.Equal(TransactionsItemSubscriptionStatus.Expired, transaction.Status);
                        Assert.Equal("eyabc", transaction.SignedTransactionInfo);
                        Assert.Equal("eyxyz", transaction.SignedRenewalInfo);
                    }
                );
            }
        );
    }

    [Fact]
    public async Task GetNotificationHistory_Success()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://local-testing-base-url/inApps/v1/notifications/history")
            .Respond("application/json", "{\"notificationHistory\":[]}");

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);
        NotificationHistoryResponse? response = await client.GetNotificationHistory(new NotificationHistoryRequest());

        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetTransactionHistory_Success()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://local-testing-base-url/inApps/v2/history/123456")
            .Respond("application/json", "{\"signedTransactions\":[]}");

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);
        TransactionHistoryResponse? response = await client.GetTransactionHistory("123456");

        Assert.NotNull(response);
    }

    [Fact]
    public async Task SendConsumptionData_Success()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://local-testing-base-url/inApps/v1/transactions/consumption/123456")
            .Respond(System.Net.HttpStatusCode.OK);

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);
        await client.SendConsumptionData(
            "123456",
            new ConsumptionRequest
            {
                AccountTenure = AccountTenure.ZeroToThreeDays,
                AppAccountToken = "test-token",
                ConsumptionStatus = ConsumptionStatus.NotConsumed,
                CustomerConsented = true,
                DeliveryStatus = DeliveryStatusV1.DeliveredAndWorkingProperly,
                LifetimeDollarsPurchased = LifetimeDollarsPurchased.ZeroDollars,
                LifetimeDollarsRefunded = LifetimeDollarsRefunded.ZeroDollars,
                Platform = Platform.Apple,
                PlayTime = PlayTime.ZeroToFiveMinutes,
                RefundPreference = RefundPreferenceV1.PreferGrant,
                SampleContentProvided = false,
                UserStatus = UserStatus.Active,
            }
        );
    }

    [Fact]
    public async Task GetTransactionInfo_Success()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://local-testing-base-url/inApps/v1/transactions/1234")
            .Respond("application/json", "{\"signedTransactionInfo\":\"signed_transaction_info_value\"}");

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);

        TransactionInfoResponse? response = await client.GetTransactionInfo("1234");
        Assert.NotNull(response);
        Assert.Equal("signed_transaction_info_value", response.SignedTransactionInfo);
    }

    [Fact]
    public async Task LookUpOrderId_Success()
    {
        const string responseData = """
            {
              "status": 1,
              "signedTransactions": [
                "signed_transaction_one",
                "signed_transaction_two"
              ]
            }
            """;

        var mockHttp = new MockHttpMessageHandler();
        mockHttp
            .When($"https://local-testing-base-url/inApps/v1/lookup/W002182")
            .Respond("application/json", responseData);

        AppStoreServerApiClient client = GetAppStoreServerApiClient(mockHttp);

        OrderLookupResponse response = await client.LookUpOrderId("W002182");

        Assert.NotNull(response);
        Assert.Equal(OrderLookupStatus.Invalid, response.Status);
        Assert.Collection(
            response.SignedTransactions,
            transaction => Assert.Equal("signed_transaction_one", transaction),
            transaction => Assert.Equal("signed_transaction_two", transaction)
        );
    }
}
