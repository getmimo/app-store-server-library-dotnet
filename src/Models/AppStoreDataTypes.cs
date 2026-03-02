using System.Text.Json.Serialization;

namespace Mimo.AppStoreServerLibrary.Models;

/// <summary>
/// The response body the App Store sends in a version 2 server notification.
/// https://developer.apple.com/documentation/appstoreservernotifications/responsebodyv2
/// </summary>
public class ResponseBodyV2
{
    /// <summary>
    /// The payload in JSON Web Signature (JWS) format, signed by the App Store.
    /// </summary>
    public string SignedPayload { get; set; } = null!;
}

/// <summary>
/// A decoded payload containing the version 2 notification data.
/// https://developer.apple.com/documentation/appstoreservernotifications/responsebodyv2decodedpayload
/// </summary>
public class ResponseBodyV2DecodedPayload
{
    /// <summary>
    /// The in-app purchase event for which the App Store sends this version 2 notification
    /// </summary>
    public string NotificationType { get; set; } = null!;

    /// <summary>
    /// Additional information that identifies the notification event. The subtype field is present only for specific version 2 notifications.
    /// </summary>
    public string Subtype { get; set; } = null!;

    /// <summary>
    /// The object that contains the app metadata and signed renewal and transaction information.
    /// The data and summary fields are mutually exclusive. The payload contains one of the fields, but not both.
    /// </summary>
    public DecodedPayloadData Data { get; set; } = null!;

    /// <summary>
    /// The summary data that appears when the App Store server completes your request to extend a subscription renewal date for eligible subscribers.
    /// </summary>
    public DecodedPayloadSummary? Summary { get; set; }

    /// <summary>
    /// The App Store Server Notification version number, "2.0".
    /// </summary>
    public string Version { get; set; } = null!;

    /// <summary>
    /// The UNIX time, in milliseconds, that the App Store signed the JSON Web Signature data.
    /// </summary>
    public long SignedDate { get; set; }

    /// <summary>
    /// A unique identifier for the notification. Use this value to identify a duplicate notification.
    /// </summary>
    [JsonPropertyName("notificationUUID")]
    public Guid NotificationUuid { get; set; }
}

/// <summary>
/// A decoded JSON Web Signature header containing transaction or renewal information.
/// https://developer.apple.com/documentation/appstoreservernotifications/jwsdecodedheader
/// </summary>
public class JWSDecodedHeader
{
    /// <summary>
    /// The algorithm used for signing the JSON Web Signature (JWS)
    /// </summary>
    public string Alg { get; set; } = null!;

    /// <summary>
    /// Notifications are signed using a certificate chain in the following order :
    /// 1. A certificate that contains the public key that corresponds to the key the App Store uses to digitally sign the JWS.
    /// 2. An Apple intermediate certificate from the Apple PKI site that starts with Worldwide Developer Relations.
    /// 3. An Apple root certificate.
    /// </summary>
    public string[] x5c { get; set; } = null!;
}

/// <summary>
/// The app metadata and the signed renewal and transaction information.
/// https://developer.apple.com/documentation/appstoreservernotifications/data
/// </summary>
public class DecodedPayloadData
{
    /// <summary>
    /// The unique identifier of the app that the notification applies to.
    /// </summary>
    public long? AppAppleId { get; set; }

    /// <summary>
    /// The bundle identifier of the app.
    /// </summary>
    public string BundleId { get; set; } = null!;

    /// <summary>
    /// The version of the build that identifies an iteration of the bundle.
    /// </summary>
    public string? BundleVersion { get; set; }

    /// <summary>
    /// The server environment that the notification applies to, either sandbox or production.
    /// </summary>
    public string Environment { get; set; } = null!;

    /// <summary>
    /// Subscription renewal information signed by the App Store, in JSON Web Signature (JWS) format. This field appears only for notifications sent for auto-renewable subscriptions.
    /// </summary>
    public string? SignedRenewalInfo { get; set; }

    /// <summary>
    /// Transaction information signed by the App Store, in JSON Web Signature (JWS) format.
    /// </summary>
    public string? SignedTransactionInfo { get; set; }
}

/// <summary>
/// The payload data for a subscription-renewal-date extension notification.
/// https://developer.apple.com/documentation/appstoreservernotifications/summary
/// </summary>
public class DecodedPayloadSummary
{
    /// <summary>
    /// The UUID that represents a specific request to extend a subscription renewal date.
    /// </summary>
    public string RequestIdentifier { get; set; } = null!;

    /// <summary>
    /// The server environment that the notification applies to, either sandbox or production.
    /// </summary>
    public string Environment { get; set; } = null!;

    /// <summary>
    /// The unique identifier of the app that the notification applies to. This property is available for apps that users download from the App Store. It isn’t present in the sandbox environment.
    /// </summary>
    public string AppAppleId { get; set; } = null!;

    /// <summary>
    /// The bundle identifier of the app.
    /// </summary>
    public string BundleId { get; set; } = null!;

    /// <summary>
    /// The product identifier of the auto-renewable subscription that the subscription-renewal-date extension applies to.
    /// </summary>
    public string ProductId { get; set; } = null!;

    /// <summary>
    /// A list of country codes that limits the App Store’s attempt to apply the subscription-renewal-date extension. If this list isn’t present, the subscription-renewal-date extension applies to all storefronts.
    /// </summary>
    public string StorefrontCountryCodes { get; set; } = null!;

    /// <summary>
    /// The final count of subscriptions that fail to receive a subscription-renewal-date extension.
    /// </summary>
    public string FailedCount { get; set; } = null!;

    /// <summary>
    /// The final count of subscriptions that successfully receive a subscription-renewal-date extension.
    /// </summary>
    public string SucceededCount { get; set; } = null!;
}

/// <summary>
/// A decoded payload that contains transaction information.
/// https://developer.apple.com/documentation/appstoreservernotifications/jwstransactiondecodedpayload
/// </summary>
public class JwsTransactionDecodedPayload
{
    /// <summary>
    /// A UUID you create at the time of purchase that associates the transaction with a customer on your own service.
    /// </summary>
    public string? AppAccountToken { get; set; } = null!;

    /// <summary>
    /// The bundle identifier of the app.
    /// </summary>
    public string BundleId { get; set; } = null!;

    /// <summary>
    /// The three-letter ISO 4217 currency code associated with the price parameter. This value is present only if price is present.
    /// </summary>
    public string Currency { get; set; } = null!;

    /// <summary>
    /// The server environment, either sandbox or production.
    /// </summary>
    public string Environment { get; set; } = null!;

    /// <summary>
    /// The UNIX time, in milliseconds, that the subscription expires or renews.
    /// </summary>
    public long ExpiresDate { get; set; }

    /// <summary>
    /// A string that describes whether the transaction was purchased by the customer, or is available to them through Family Sharing.
    /// </summary>
    public string InAppOwnershipType { get; set; } = null!;

    /// <summary>
    /// A Boolean value that indicates whether the customer upgraded to another subscription.
    /// </summary>
    public bool? IsUpgraded { get; set; }

    /// <summary>
    /// The payment mode the subscription offer uses, such as Free Trial, Pay As You Go, or Pay Up Front.
    /// </summary>
    public string? OfferDiscountType { get; set; }

    /// <summary>
    /// The identifier that contains the offer code or the promotional offer identifier.
    /// </summary>
    public string? OfferIdentifier { get; set; }

    /// <summary>
    /// A value that represents the promotional offer type.
    /// </summary>
    public int OfferType { get; set; }

    /// <summary>
    /// The UNIX time, in milliseconds, that represents the purchase date of the original transaction identifier.
    /// </summary>
    public long OriginalPurchaseDate { get; set; }

    /// <summary>
    /// The transaction identifier of the original purchase.
    /// </summary>
    public string OriginalTransactionId { get; set; } = null!;

    /// <summary>
    /// A value that represents the price multiplied by 1000 of the in-app purchase or subscription offer you configured in App Store Connect and that the system records at the time of the purchase.
    /// Apple documentation declares it's an Integer but it can actually hold a greater value.
    /// We have chosen to use a double as it's already the type chosen to store the price in the `AppleProduct` table.
    /// </summary>
    public long Price { get; set; }

    /// <summary>
    /// The product identifier of the in-app purchase.
    /// </summary>
    public string ProductId { get; set; } = null!;

    /// <summary>
    /// The UNIX time, in milliseconds, that the App Store charged the user’s account for a purchase, restored product, subscription, or subscription renewal after a lapse.
    /// </summary>
    public long PurchaseDate { get; set; }

    /// <summary>
    /// The number of consumable products the user purchased.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The UNIX time, in milliseconds, that the App Store refunded the transaction or revoked it from Family Sharing.
    /// </summary>
    public long RevocationDate { get; set; }

    /// <summary>
    /// The reason that the App Store refunded the transaction or revoked it from Family Sharing.
    /// Values :
    /// 0 : The App Store refunded the transaction on behalf of the customer for other reasons, for example, an accidental purchase.
    /// 1 : The App Store refunded the transaction on behalf of the customer due to an actual or perceived issue within your app.
    /// </summary>
    public int? RevocationReason { get; set; }

    /// <summary>
    /// The UNIX time, in milliseconds, that the App Store signed the JSON Web Signature (JWS) data.
    /// </summary>
    public long SignedDate { get; set; }

    /// <summary>
    /// The three-letter code that represents the country or region associated with the App Store storefront for the purchase.
    /// </summary>
    public string Storefront { get; set; } = null!;

    /// <summary>
    /// An Apple-defined value that uniquely identifies the App Store storefront associated with the purchase.
    /// </summary>
    public string StorefrontId { get; set; } = null!;

    /// <summary>
    /// The identifier of the subscription group to which the subscription belongs.
    /// </summary>
    public string? SubscriptionGroupIdentifier { get; set; }

    /// <summary>
    /// The unique identifier of the transaction.
    /// </summary>
    public string TransactionId { get; set; } = null!;

    /// <summary>
    /// The reason for the purchase transaction, which indicates whether it’s a customer’s purchase or a renewal for an auto-renewable subscription that the system initiates.
    /// </summary>
    public string? TransactionReason { get; set; }

    /// <summary>
    /// The type of the in-app purchase.
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// The unique identifier of subscription purchase events across devices, including subscription renewals.
    /// </summary>
    public string? WebOrderLineItemId { get; set; }
}

/// <summary>
/// A decoded payload containing subscription renewal information for an auto-renewable subscription.
/// https://developer.apple.com/documentation/appstoreservernotifications/jwsrenewalinfodecodedpayload
/// </summary>
public class JWSRenewalInfoDecodedPayload
{
    /// <summary>
    /// The product identifier of the product that renews at the next billing period.
    /// </summary>
    public string AutoRenewProductId { get; set; } = null!;

    /// <summary>
    /// The renewal status for an auto-renewable subscription.
    /// 0 : Automatic renewal is off.
    /// 1 : Automatic renewal is on.
    /// </summary>
    public int AutoRenewStatus { get; set; }

    /// <summary>
    /// The server environment, either sandbox or production.
    /// </summary>
    public string Environment { get; set; } = null!;

    /// <summary>
    /// The reason a subscription expired.
    /// Values could be :
    /// 1 : The customer canceled their subscription.
    /// 2 : Billing error; for example, the customer’s payment information is no longer valid.
    /// 3 : The customer didn’t consent to an auto-renewable subscription price increase that requires customer consent, allowing the subscription to expire.
    /// 4 : The product wasn’t available for purchase at the time of renewal.
    /// 5 : The subscription expired for some other reason.
    /// </summary>
    public int? ExpirationIntent { get; set; }

    /// <summary>
    /// The time when the billing grace period for subscription renewals expires.
    /// </summary>
    public long GracePeriodExpiresDate { get; set; }

    /// <summary>
    /// The Boolean value that indicates whether the App Store is attempting to automatically renew an expired subscription.
    /// </summary>
    public bool IsInBillingRetryPeriod { get; set; }

    /// <summary>
    /// The offer code or the promotional offer identifier.
    /// </summary>
    public string? OfferIdentifier { get; set; }

    /// <summary>
    /// The type of subscription offer.
    /// </summary>
    public int OfferType { get; set; }

    /// <summary>
    /// The original transaction identifier of a purchase.
    /// </summary>
    public string OriginalTransactionId { get; set; } = null!;

    /// <summary>
    /// The status that indicates whether the auto-renewable subscription is subject to a price increase.
    /// </summary>
    public int PriceIncreaseStatus { get; set; }

    /// <summary>
    /// The product identifier of the in-app purchase.
    /// </summary>
    public string ProductId { get; set; } = null!;

    /// <summary>
    /// The earliest start date of an auto-renewable subscription in a series of subscription purchases that ignores all lapses of paid service that are 60 days or less.
    /// </summary>
    public long RecentSubscriptionStartDate { get; set; }

    /// <summary>
    /// The UNIX time, in milliseconds, that the most recent auto-renewable subscription purchase expires.
    /// </summary>
    public long RenewalDate { get; set; }

    /// <summary>
    /// The UNIX time, in milliseconds, that the App Store signed the JSON Web Signature (JWS) data.
    /// </summary>
    public long SignedDate { get; set; }
}

/// <summary>
/// A response that contains status information for all of a customer’s auto-renewable subscriptions in your app.
/// https://developer.apple.com/documentation/appstoreserverapi/statusresponse
/// </summary>
public class SubscriptionStatusResponse
{
    /// <summary>
    /// An array of information for auto-renewable subscriptions, including App Store-signed transaction information and App Store-signed renewal information.
    /// </summary>
    public List<SubscriptionStatusGroupIdentifierItem> Data { get; set; } = new();

    /// <summary>
    /// The unique identifier of the app that the notification applies to.
    /// </summary>
    public long AppAppleId { get; set; }

    /// <summary>
    /// The bundle identifier of the app.
    /// </summary>
    public string BundleId { get; set; } = null!;

    /// <summary>
    /// The server environment that the notification applies to, either sandbox or production.
    /// </summary>
    public string Environment { get; set; } = null!;
}

/// <summary>
/// Information for auto-renewable subscriptions, including signed transaction information and signed renewal information, for one subscription group.
/// https://developer.apple.com/documentation/appstoreserverapi/subscriptiongroupidentifieritem
/// </summary>
public class SubscriptionStatusGroupIdentifierItem
{
    /// <summary>
    /// The identifier of the subscription group that the subscription belongs to.
    /// </summary>
    public string SubscriptionGroupIdentifier { get; set; } = null!;

    /// <summary>
    /// An array of the most recent App Store-signed transaction information and App Store-signed renewal information for all auto-renewable subscriptions in the subscription group.
    /// </summary>
    public List<SubscriptionStatusLastTransactionsItem> LastTransactions { get; set; } = new();
}

/// <summary>
/// The most recent App Store-signed transaction information and App Store-signed renewal information for an auto-renewable subscription.
/// https://developer.apple.com/documentation/appstoreserverapi/lasttransactionsitem
/// </summary>
public class SubscriptionStatusLastTransactionsItem
{
    /// <summary>
    /// The original transaction identifier of the auto-renewable subscription.
    /// </summary>
    public string OriginalTransactionId { get; set; } = null!;

    /// <summary>
    /// The status of the auto-renewable subscription.
    /// 1 - The auto-renewable subscription is active.
    /// 2 - The auto-renewable subscription is expired.
    /// 3 - The auto-renewable subscription is in a billing retry period.
    /// 4 - The auto-renewable subscription is in a Billing Grace Period.
    /// 5 - The auto-renewable subscription is revoked. The App Store refunded the transaction or revoked it from Family Sharing.
    /// </summary>
    public TransactionsItemSubscriptionStatus Status { get; set; }

    /// <summary>
    /// Subscription renewal information signed by the App Store, in JSON Web Signature (JWS) format. This field appears only for notifications sent for auto-renewable subscriptions.
    /// </summary>
    public string? SignedRenewalInfo { get; set; }

    /// <summary>
    /// Transaction information signed by the App Store, in JSON Web Signature (JWS) format.
    /// </summary>
    public string? SignedTransactionInfo { get; set; }
}

public enum TransactionsItemSubscriptionStatus
{
    /// <summary>
    /// The auto-renewable subscription is active.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The auto-renewable subscription is expired.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// The auto-renewable subscription is in a billing retry period.
    /// </summary>
    BillingRetryPeriod = 3,

    /// <summary>
    /// The auto-renewable subscription is in a Billing Grace Period.
    /// </summary>
    BillingGracePeriod = 4,

    /// <summary>
    /// The auto-renewable subscription is revoked.
    /// </summary>
    Revoked = 5,
}

/// <summary>
/// The request body for notification history.
/// https://developer.apple.com/documentation/appstoreserverapi/notificationhistoryrequest
/// </summary>
public class NotificationHistoryRequest
{
    /// <summary>
    /// Required. The start date of the timespan for the requested App Store Server Notification history records.
    /// </summary>
    public long StartDate { get; set; }

    /// <summary>
    /// Required. The end date of the timespan for the requested App Store Server Notification history records.
    /// </summary>
    public long EndDate { get; set; }

    /// <summary>
    /// Optional. A notification type. Provide this field to limit the notification history records to those with this one notification type.
    /// </summary>
    public string? NotificationType { get; set; }

    /// <summary>
    /// Optional. A notification subtype. Provide this field to limit the notification history records to those with this one notification subtype.
    /// If you specify a notificationSubtype, you need to also specify its related notificationType.
    /// </summary>
    public string? NotificationSubType { get; set; }

    /// <summary>
    /// Optional. A Boolean value you set to true to request only the notifications that haven’t reached your server successfully.
    /// The response also includes notifications that the App Store server is currently retrying to send to your server.
    /// </summary>
    public bool? OnlyFailures { get; set; }

    /// <summary>
    /// Optional. The transaction identifier, which may be an original transaction identifier, of any transaction belonging to the customer.
    /// Provide this field to limit the notification history request to this one customer.
    /// </summary>
    public string? TransactionId { get; set; }
}

/// <summary>
/// A response that contains the App Store Server Notifications history for your app.
/// https://developer.apple.com/documentation/appstoreserverapi/notificationhistoryresponse
/// </summary>
public class NotificationHistoryResponse
{
    /// <summary>
    /// An array of App Store Server Notifications history records.
    /// </summary>
    public List<NotificationHistoryResponseItem> NotificationHistory { get; set; } = new();

    /// <summary>
    /// A Boolean value that indicates whether the App Store has more notification history records to send.
    /// If hasMore is true, use the paginationToken in the subsequent request to get more records.
    /// If hasMore is false, there are no more records available.
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// A pagination token that you provide to Get Notification History on a subsequent request to get the next page of responses.
    /// </summary>
    public string PaginationToken { get; set; } = null!;
}

/// <summary>
/// The request body for notification history.
/// https://developer.apple.com/documentation/appstoreserverapi/notificationhistoryrequest
/// </summary>
public class NotificationHistoryResponseItem
{
    /// <summary>
    /// An array of information the App Store server records for its attempts to send a notification to your server.
    /// The maximum number of entries in the array is six.
    /// </summary>
    public List<SendAttemptItem> SendAttempts { get; set; } = new();

    /// <summary>
    /// The cryptographically signed payload, in JSON Web Signature (JWS) format, containing the original response body of a version 2 notification.
    /// </summary>
    public string SignedPayload { get; set; } = null!;
}

/// <summary>
/// The request body for notification history.
/// https://developer.apple.com/documentation/appstoreserverapi/notificationhistoryrequest
/// </summary>
public class SendAttemptItem
{
    /// <summary>
    /// The date the App Store server attempts to send the notification.
    /// </summary>
    public long AttemptDate { get; set; }

    /// <summary>
    /// The success or error information the App Store server records when it attempts to send an App Store server notification to your server.
    /// See possible values : https://developer.apple.com/documentation/appstoreserverapi/sendattemptresult
    /// </summary>
    public string SendAttemptResult { get; set; } = null!;
}

/// <summary>
/// A response that contains the customer’s transaction history for an app.
/// </summary>
public class TransactionHistoryResponse
{
    public string AppAppleId { get; set; } = null!;
    public string BundleId { get; set; } = null!;
    public string Environment { get; set; } = null!;
    public bool HasMore { get; set; }
    public string Revision { get; set; } = null!;
    public List<string> SignedTransactions { get; set; } = null!;
}

/// <summary>
/// A response that contains the customer’s transaction information for an app.
/// </summary>
public class TransactionInfoResponse
{
    public string SignedTransactionInfo { get; set; } = null!;
}

/// <summary>
/// A response that includes the order lookup status and an array of signed transactions for the in-app purchases in the order.
/// </summary>
public class OrderLookupResponse
{
    public OrderLookupStatus Status { get; set; }
    public List<string> SignedTransactions { get; set; } = null!;
}

/// <summary>
/// A value that indicates whether the order ID in the request is valid for your app.
/// </summary>
public enum OrderLookupStatus
{
    /// <summary>
    /// Valid
    /// </summary>
    Valid = 0,

    /// <summary>
    /// Invalid
    /// </summary>
    Invalid = 1,
}

/// <summary>
/// Error response from the App Store Server API.
/// </summary>
public class ErrorResponse
{
    public int ErrorCode { get; set; } = 0;
    public string ErrorMessage { get; set; } = null!;
}
