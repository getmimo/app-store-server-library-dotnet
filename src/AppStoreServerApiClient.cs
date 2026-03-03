using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Mimo.AppStoreServerLibrary.Exceptions;
using Mimo.AppStoreServerLibrary.Models;

namespace Mimo.AppStoreServerLibrary;

/// <summary>
/// Create an App Store Server API client
/// </summary>
/// <param name="signingKey">Your private key downloaded from App Store Connect</param>
/// <param name="keyId">Your private key ID from App Store Connect</param>
/// <param name="issuerId">Your issuer ID from the Keys page in App Store Connect</param>
/// <param name="bundleId">Your app's bundle ID</param>
/// <param name="environment">The environment to target</param>
/// <param name="httpClient">An optional HttpClient instance to use for requests</param>
public class AppStoreServerApiClient(
    string signingKey,
    string keyId,
    string issuerId,
    string bundleId,
    AppStoreEnvironment environment,
    HttpClient? httpClient = null
)
{
    private static readonly Lazy<HttpClient> DefaultHttpClient = new(() => new HttpClient());
    private readonly HttpClient httpClient = httpClient ?? DefaultHttpClient.Value;

    /// <summary>
    /// Get the statuses for all of a customer’s auto-renewable subscriptions in your app.
    /// </summary>
    /// <param name="transactionId"> The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier</param>
    /// <returns>The status for all the customer’s subscriptions, organized by their subscription group identifier.</returns>
    public Task<SubscriptionStatusResponse> GetAllSubscriptionStatuses(string transactionId)
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_all_subscription_statuses

        string path = $"/inApps/v1/subscriptions/{transactionId}";

        return this.MakeRequest<SubscriptionStatusResponse>(path, HttpMethod.Get)!;
    }

    /// <summary>
    /// Get a list of notifications that the App Store server attempted to send to your server.
    /// </summary>
    /// <returns>A list of notifications and their attempts</returns>
    public Task<NotificationHistoryResponse?> GetNotificationHistory(
        NotificationHistoryRequest notificationHistoryRequest,
        string paginationToken = ""
    )
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_notification_history
        Dictionary<string, string> queryParameters = new();
        if (!string.IsNullOrEmpty(paginationToken))
        {
            queryParameters.Add("paginationToken", paginationToken);
        }

        const string path = "/inApps/v1/notifications/history";

        return this.MakeRequest<NotificationHistoryResponse>(
            path,
            HttpMethod.Post,
            queryParameters,
            notificationHistoryRequest
        );
    }

    /// <summary>
    /// Get a customer’s in-app purchase transaction history for your app.
    /// </summary>
    /// <returns>A list of transactions associated with the provided Transaction ID</returns>
    public Task<TransactionHistoryResponse?> GetTransactionHistory(string transactionId, string revisionToken = "")
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_transaction_history
        Dictionary<string, string> queryParameters = new();
        if (!string.IsNullOrEmpty(revisionToken))
        {
            queryParameters.Add("revision", revisionToken);
        }

        string path = $"/inApps/v2/history/{transactionId}";

        return this.MakeRequest<TransactionHistoryResponse>(path, HttpMethod.Get, queryParameters);
    }

    /// <summary>
    /// Send consumption information about a consumable in-app purchase to the App Store after your server receives a consumption request notification.
    /// </summary>
    /// <param name="transactionId">The transaction identifier for which you're providing consumption information. You receive this identifier in the CONSUMPTION_REQUEST notification the App Store sends to your server.</param>
    /// <param name="consumptionRequest">The request body containing consumption information.</param>
    /// <exception cref="ApiException">Thrown when a response indicates the request could not be processed</exception>
    /// <remarks>
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/send_consumption_information">Send Consumption Information</see>
    /// </remarks>
    public Task SendConsumptionData(string transactionId, ConsumptionRequest consumptionRequest)
    {
        string path = $"/inApps/v1/transactions/consumption/{transactionId}";

        return this.MakeRequest<object?>(path, HttpMethod.Put, null, consumptionRequest, false);
    }

    /// <summary>
    /// Send consumption information about an in-app purchase to the App Store after your server receives a consumption request notification.
    /// </summary>
    /// <param name="transactionId">The transaction identifier for which you're providing consumption information. You receive this identifier in the CONSUMPTION_REQUEST notification the App Store sends to your server's App Store Server Notifications V2 endpoint.</param>
    /// <param name="consumptionRequest">The request body containing consumption information.</param>
    /// <exception cref="ApiException">Thrown when a response indicates the request could not be processed</exception>
    /// <remarks>
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/send-consumption-information">Send Consumption Information</see>
    /// </remarks>
    public Task SendConsumptionDataV2(string transactionId, ConsumptionRequestV2 consumptionRequest)
    {
        string path = $"/inApps/v2/transactions/consumption/{transactionId}";

        return this.MakeRequest<object?>(path, HttpMethod.Put, null, consumptionRequest, false);
    }

    /// <summary>
    /// Get information about a single transaction for your app.
    /// </summary>
    /// <param name="transactionId">The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier.</param>
    /// <exception cref="ApiException">Thrown when a response indicates the request could not be processed</exception>
    /// <remarks>
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/get-v1-transactions-_transactionid_">Get Transaction Info</see>
    /// </remarks>
    public Task<TransactionInfoResponse?> GetTransactionInfo(string transactionId)
    {
        string path = $"/inApps/v1/transactions/{transactionId}";

        return this.MakeRequest<TransactionInfoResponse>(path, HttpMethod.Get);
    }

    /// <summary>
    /// Get a customer’s in-app purchases from a receipt using the order ID.
    /// </summary>
    /// <param name="orderId">The order ID for in-app purchases that belong to the customer.</param>
    /// <exception cref="ApiException">Thrown when a response indicates the request could not be processed</exception>
    /// <remarks>
    /// See <see href="https://developer.apple.com/documentation/appstoreserverapi/get-v1-lookup-_orderid_">Look Up Order ID</see>
    /// </remarks>
    public Task<OrderLookupResponse> LookUpOrderId(string orderId)
    {
        string path = $"/inApps/v1/lookup/{orderId}";

        return this.MakeRequest<OrderLookupResponse>(path, HttpMethod.Get)!;
    }

    /// <summary>
    /// Returns a signed JWT token that can be used to make requests to the App Store Server API directly.
    /// </summary>
    public string GetApiToken()
    {
        var prvKey = ECDsa.Create();
        prvKey.ImportFromPem(signingKey);

        var securityDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(1),
            Claims = new Dictionary<string, object>
            {
                { "iss", issuerId },
                { "aud", "appstoreconnect-v1" },
                { "bid", bundleId },
            },
            TokenType = "JWT",
        };

        var securityKey = new ECDsaSecurityKey(prvKey) { KeyId = keyId };

        securityDescriptor.SigningCredentials = new SigningCredentials(securityKey, "ES256");

        return new JsonWebTokenHandler().CreateToken(securityDescriptor);
    }

    private async Task<TReturn?> MakeRequest<TReturn>(
        string path,
        HttpMethod method,
        Dictionary<string, string>? queryParameters = null,
        object? body = null,
        bool fetchResponse = true
    )
        where TReturn : class
    {
        string token = this.GetApiToken();

        UriBuilder builder = new(environment.BaseUrl) { Path = path };

        if (queryParameters != null && queryParameters.Any())
        {
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);

            foreach (KeyValuePair<string, string> param in queryParameters)
            {
                query[param.Key] = param.Value;
            }

            builder.Query = query.ToString();
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        try
        {
            HttpResponseMessage httpResponse;

            if (method == HttpMethod.Get)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpResponse = await this.httpClient.SendAsync(request);
            }
            else if (method == HttpMethod.Post)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body, jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );
                httpResponse = await this.httpClient.SendAsync(request);
            }
            else if (method == HttpMethod.Put)
            {
                var request = new HttpRequestMessage(HttpMethod.Put, builder.Uri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body, jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );
                httpResponse = await this.httpClient.SendAsync(request);
            }
            else
            {
                throw new NotSupportedException($"Method {method} not supported");
            }

            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                return fetchResponse ? JsonSerializer.Deserialize<TReturn>(responseContent, jsonOptions) : null;
            }

            var error = JsonSerializer.Deserialize<ErrorResponse>(responseContent, jsonOptions);

            throw new ApiException(httpResponse.StatusCode, error);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException(ex.StatusCode, null, ex);
        }
    }
}
