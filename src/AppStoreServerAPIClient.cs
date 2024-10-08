using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Mimo.AppStoreServerLibraryDotnet.Configuration;
using Mimo.AppStoreServerLibraryDotnet.Exceptions;
using Mimo.AppStoreServerLibraryDotnet.Models;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Mimo.AppStoreServerLibraryDotnet;

public class AppStoreServerAPIClient(
    IOptions<AppleOptions> appleOptions)
    : IAppStoreServerAPIClient
{
    /// <summary>
    /// Returns a signed JSON Web Token for Calling App Store Server API.
    /// </summary>
    /// <param name="keyId">Your private key ID from App Store Connect</param>
    /// <param name="issuerId">Your issuer ID from the API Keys page in App Store Connect</param>
    /// <param name="privateKey">The private that was generated by Apple, encoded as Base64</param>
    /// <param name="bundleId">Your App bundle Id</param>
    /// <returns>Signed JWT</returns>
    public string GetAppStoreServerApiToken(string keyId, string issuerId, string privateKey, string bundleId)
    {
        if(string.IsNullOrEmpty(keyId))
        {
            throw new ArgumentNullException(nameof(keyId), "AppStoreServerApiKeyId was not provided. Please check your configuration.");
        }

        if(string.IsNullOrEmpty(issuerId))
        {
            throw new ArgumentNullException(nameof(issuerId), "AppStoreServerApiIssuerId was not provided. Please check your configuration.");
        }

        if(string.IsNullOrEmpty(privateKey))
        {
            throw new ArgumentNullException(nameof(privateKey), "AppStoreServerApiSubscriptionKey was not provided. Please check your configuration.");
        }

        if (!Base64.IsValid(privateKey))
        {
            throw new ArgumentNullException(nameof(privateKey), $"AppStoreServerApiSubscriptionKey is not a valid Base64 string");
        }
        
        ReadOnlySpan<byte> keyAsSpan = Convert.FromBase64String(privateKey);
        var prvKey = ECDsa.Create();
        prvKey.ImportPkcs8PrivateKey(keyAsSpan, out int _);

        var securityDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(1),
            Claims = new Dictionary<string, object>
            {
                { "iss", issuerId },
                { "aud", "appstoreconnect-v1" },
                { "bid", bundleId }
            },
            TokenType = "JWT"
        };

        var signingKey = new ECDsaSecurityKey(prvKey)
        {
            KeyId = keyId
        };

        securityDescriptor.SigningCredentials = new SigningCredentials(signingKey, "ES256");

        return new JsonWebTokenHandler().CreateToken(securityDescriptor);
    }

    /// <summary>
    /// Get the statuses for all of a customer’s auto-renewable subscriptions in your app.
    /// </summary>
    /// <param name="transactionId"> The identifier of a transaction that belongs to the customer, and which may be an original transaction identifier</param>
    /// <returns>The status for all the customer’s subscriptions, organized by their subscription group identifier.</returns>
    public async Task<SubscriptionStatusResponse> GetAllSubscriptionStatuses(string transactionId)
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_all_subscription_statuses

        string path = $"v1/subscriptions/{transactionId}";

        return (await MakeRequest<SubscriptionStatusResponse>(path, HttpMethod.Get))!;
    }

    /// <summary>
    /// Get a list of notifications that the App Store server attempted to send to your server.
    /// </summary>
    /// <returns>A list of notifications and their attempts</returns>
    public async Task<NotificationHistoryResponse?> GetNotificationHistory(NotificationHistoryRequest notificationHistoryRequest,
        string paginationToken = "")
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_notification_history
        Dictionary<string, string> queryParameters = new();
        if (!string.IsNullOrEmpty(paginationToken))
        {
            queryParameters.Add("paginationToken", paginationToken);
        }

        string path = $"v1/notifications/history";

        return await this.MakeRequest<NotificationHistoryResponse>(path, HttpMethod.Post, queryParameters, notificationHistoryRequest);
    }

    /// <summary>
    /// Get a customer’s in-app purchase transaction history for your app.
    /// </summary>
    /// <returns>A list of transactions associated with the provided Transaction Id</returns>
    public async Task<TransactionHistoryResponse?> GetTransactionHistory(string transactionId,
        string revisionToken = "")
    {
        //Call to https://developer.apple.com/documentation/appstoreserverapi/get_transaction_history
        Dictionary<string, string> queryParameters = new();
        if (!string.IsNullOrEmpty(revisionToken))
        {
            queryParameters.Add("revision", revisionToken);
        }

        string path = $"v2/history/{transactionId}";

        return await this.MakeRequest<TransactionHistoryResponse>(path, HttpMethod.Get, queryParameters);
    }

    /// <summary>
    /// Call the App Store Server API
    /// </summary>
    /// <param name="path">Endpoint you need to call</param>
    /// <param name="method">Http Method : Get, Post, etc.</param>
    /// <param name="queryParameters">Any query param you need to append</param>
    /// <param name="body">Query body if required</param>
    /// <typeparam name="TReturn">The type to deserialize the API response to</typeparam>
    /// <exception cref="NotSupportedException">Supports only Get and Post http methods</exception>
    private async Task<TReturn?> MakeRequest<TReturn>(string path, HttpMethod method, Dictionary<string, string>? queryParameters = null,
        object? body = null)
    {
        string baseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production"
            ? "https://api.storekit.itunes.apple.com/inApps"
            : "https://api.storekit-sandbox.itunes.apple.com/inApps";

        string token = GetAppStoreServerApiToken(appleOptions.Value.AppStoreServerApiKeyId,
            appleOptions.Value.AppStoreServerApiIssuerId,
            appleOptions.Value.AppStoreServerApiSubscriptionKey,
            appleOptions.Value.BundleId);

        string url = $"{baseUrl}/{path}";

        TReturn? response;
        try
        {
            IFlurlRequest request = url
                .WithOAuthBearerToken(token)
                .WithSettings(settings => settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

            //Join the query parameters to the request
            if (queryParameters != null && queryParameters.Any())
            {
                foreach (KeyValuePair<string, string> queryParameter in queryParameters)
                {
                    request.SetQueryParam(queryParameter.Key, queryParameter.Value);
                }
            }

            if (method == HttpMethod.Get)
            {
                response = await request
                    .GetAsync()
                    .ReceiveJson<TReturn>();
            }
            else if (method == HttpMethod.Post)
            {
                response = await request
                    .PostJsonAsync(body)
                    .ReceiveJson<TReturn>();
            }
            else
            {
                throw new NotSupportedException($"Method {method} not supported");
            }
        }
        catch (FlurlHttpException ex)
        {
            var error = await ex.GetResponseJsonAsync<ErrorResponse>();

            if (error != null)
            {
                throw new AppStoreServerApiException($"Error when calling App Store Server API for endpoint {path}. Received error code: {error.ErrorCode}, Received error message: {error.ErrorMessage}",
                    error);
            }

            //If the error is not in the expected format, rethrow
            throw;
        }

        return response;
    }
}