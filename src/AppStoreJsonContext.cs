using System.Text.Json.Serialization;
using Mimo.AppStoreServerLibrary.Models;

namespace Mimo.AppStoreServerLibrary;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ResponseBodyV2DecodedPayload))]
[JsonSerializable(typeof(JWSDecodedHeader))]
[JsonSerializable(typeof(JwsTransactionDecodedPayload))]
[JsonSerializable(typeof(JWSRenewalInfoDecodedPayload))]
[JsonSerializable(typeof(SubscriptionStatusResponse))]
[JsonSerializable(typeof(NotificationHistoryRequest))]
[JsonSerializable(typeof(NotificationHistoryResponse))]
[JsonSerializable(typeof(TransactionHistoryResponse))]
[JsonSerializable(typeof(TransactionInfoResponse))]
[JsonSerializable(typeof(OrderLookupResponse))]
[JsonSerializable(typeof(ConsumptionRequest))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class AppStoreJsonContext : JsonSerializerContext;
