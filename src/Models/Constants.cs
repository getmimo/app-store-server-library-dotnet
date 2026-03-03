/// <summary>
/// The reason the customer requested the refund.
/// </summary>
public static class ConsumptionRequestReason
{
    public const string UnintendedPurchase = "UNINTENDED_PURCHASE";
    public const string FulfillmentIssue = "FULFILLMENT_ISSUE";
    public const string UnsatisfiedWithPurchase = "UNSATISFIED_WITH_PURCHASE";
    public const string Legal = "LEGAL";
    public const string Other = "OTHER";
}

/// <summary>
/// The delivery status of a consumable in-app purchase (v2 string-based).
/// </summary>
public static class DeliveryStatus
{
    public const string Delivered = "DELIVERED";
    public const string UndeliveredQualityIssue = "UNDELIVERED_QUALITY_ISSUE";
    public const string UndeliveredWrongItem = "UNDELIVERED_WRONG_ITEM";
    public const string UndeliveredServerOutage = "UNDELIVERED_SERVER_OUTAGE";
    public const string UndeliveredOther = "UNDELIVERED_OTHER";
}

/// <summary>
/// The result of the first notification send attempt.
/// </summary>
public static class FirstSendAttemptResult
{
    public const string Success = "SUCCESS";
    public const string TimedOut = "TIMED_OUT";
    public const string TlsIssue = "TLS_ISSUE";
    public const string CircularRedirect = "CIRCULAR_REDIRECT";
    public const string NoResponse = "NO_RESPONSE";
    public const string SocketIssue = "SOCKET_ISSUE";
    public const string UnsupportedCharset = "UNSUPPORTED_CHARSET";
    public const string InvalidResponse = "INVALID_RESPONSE";
    public const string PrematureClose = "PREMATURE_CLOSE";
    public const string UnsuccessfulHttpResponseCode = "UNSUCCESSFUL_HTTP_RESPONSE_CODE";
    public const string Other = "OTHER";
}

/// <summary>
/// The state of an image used in a message.
/// </summary>
public static class ImageState
{
    public const string PendingReview = "PENDING_REVIEW";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
}

/// <summary>
/// A string that describes whether the transaction was purchased by the customer, or is available to them through Family Sharing.
/// </summary>
public static class InAppOwnershipType
{
    public const string FamilyShared = "FAMILY_SHARED";
    public const string Purchased = "PURCHASED";
}

/// <summary>
/// The state of a message.
/// </summary>
public static class MessageState
{
    public const string PendingReview = "PENDING_REVIEW";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
}

/// <summary>
/// The type that describes the in-app purchase event for which the App Store sends the version 2 notification.
/// </summary>
public static class NotificationTypeV2
{
    public const string Subscribed = "SUBSCRIBED";
    public const string DidChangeRenewalPref = "DID_CHANGE_RENEWAL_PREF";
    public const string DidChangeRenewalStatus = "DID_CHANGE_RENEWAL_STATUS";
    public const string OfferRedeemed = "OFFER_REDEEMED";
    public const string DidRenew = "DID_RENEW";
    public const string Expired = "EXPIRED";
    public const string DidFailToRenew = "DID_FAIL_TO_RENEW";
    public const string GracePeriodExpired = "GRACE_PERIOD_EXPIRED";
    public const string PriceIncrease = "PRICE_INCREASE";
    public const string Refund = "REFUND";
    public const string RefundDeclined = "REFUND_DECLINED";
    public const string ConsumptionRequest = "CONSUMPTION_REQUEST";
    public const string RenewalExtended = "RENEWAL_EXTENDED";
    public const string Revoke = "REVOKE";
    public const string Test = "TEST";
    public const string RenewalExtension = "RENEWAL_EXTENSION";
    public const string RefundReversed = "REFUND_REVERSED";
    public const string ExternalPurchaseToken = "EXTERNAL_PURCHASE_TOKEN";
    public const string OneTimeCharge = "ONE_TIME_CHARGE";
    public const string RescindConsent = "RESCIND_CONSENT";
}

/// <summary>
/// The payment mode of a discount offer.
/// </summary>
public static class OfferDiscountType
{
    public const string FreeTrial = "FREE_TRIAL";
    public const string PayAsYouGo = "PAY_AS_YOU_GO";
    public const string PayUpFront = "PAY_UP_FRONT";
    public const string OneTime = "ONE_TIME";
}

/// <summary>
/// The platform on which the customer made a purchase.
/// </summary>
public static class PurchasePlatform
{
    public const string iOS = "iOS";
    public const string MacOS = "macOS";
    public const string TvOS = "tvOS";
    public const string VisionOS = "visionOS";
}

/// <summary>
/// A value that indicates your preferred outcome for the refund request (v2 string-based).
/// </summary>
public static class RefundPreference
{
    public const string Decline = "DECLINE";
    public const string GrantFull = "GRANT_FULL";
    public const string GrantProrated = "GRANT_PRORATED";
}

/// <summary>
/// The type of revocation for a transaction.
/// </summary>
public static class RevocationType
{
    public const string RefundFull = "REFUND_FULL";
    public const string RefundProrated = "REFUND_PRORATED";
    public const string FamilyRevoke = "FAMILY_REVOKE";
}

/// <summary>
/// The result of a notification send attempt.
/// </summary>
public static class SendAttemptResult
{
    public const string Success = "SUCCESS";
    public const string TimedOut = "TIMED_OUT";
    public const string TlsIssue = "TLS_ISSUE";
    public const string CircularRedirect = "CIRCULAR_REDIRECT";
    public const string NoResponse = "NO_RESPONSE";
    public const string SocketIssue = "SOCKET_ISSUE";
    public const string UnsupportedCharset = "UNSUPPORTED_CHARSET";
    public const string InvalidResponse = "INVALID_RESPONSE";
    public const string PrematureClose = "PREMATURE_CLOSE";
    public const string UnsuccessfulHttpResponseCode = "UNSUCCESSFUL_HTTP_RESPONSE_CODE";
    public const string Other = "OTHER";
}

/// <summary>
/// Additional information that identifies the notification event.
/// </summary>
public static class NotificationSubtype
{
    public const string InitialBuy = "INITIAL_BUY";
    public const string Resubscribe = "RESUBSCRIBE";
    public const string Downgrade = "DOWNGRADE";
    public const string Upgrade = "UPGRADE";
    public const string AutoRenewEnabled = "AUTO_RENEW_ENABLED";
    public const string AutoRenewDisabled = "AUTO_RENEW_DISABLED";
    public const string Voluntary = "VOLUNTARY";
    public const string BillingRetry = "BILLING_RETRY";
    public const string PriceIncrease = "PRICE_INCREASE";
    public const string GracePeriod = "GRACE_PERIOD";
    public const string Pending = "PENDING";
    public const string Accepted = "ACCEPTED";
    public const string BillingRecovery = "BILLING_RECOVERY";
    public const string ProductNotForSale = "PRODUCT_NOT_FOR_SALE";
    public const string Summary = "SUMMARY";
    public const string Failure = "FAILURE";
    public const string Unreported = "UNREPORTED";
}

/// <summary>
/// The reason for a purchase transaction.
/// </summary>
public static class TransactionReason
{
    public const string Purchase = "PURCHASE";
    public const string Renewal = "RENEWAL";
}

/// <summary>
/// The type of in-app purchase products.
/// </summary>
public static class TransactionType
{
    public const string AutoRenewableSubscription = "Auto-Renewable Subscription";
    public const string NonConsumable = "Non-Consumable";
    public const string Consumable = "Consumable";
    public const string NonRenewingSubscription = "Non-Renewing Subscription";
}

/// <summary>
/// The product type for filtering transaction history.
/// </summary>
public static class ProductType
{
    public const string AutoRenewable = "AUTO_RENEWABLE";
    public const string NonRenewable = "NON_RENEWABLE";
    public const string Consumable = "CONSUMABLE";
    public const string NonConsumable = "NON_CONSUMABLE";
}

/// <summary>
/// The sort order for transaction history.
/// </summary>
public static class SortOrder
{
    public const string Ascending = "ASCENDING";
    public const string Descending = "DESCENDING";
}

/// <summary>
/// The version of the Get Transaction History API.
/// </summary>
public static class GetTransactionHistoryVersion
{
    public const string V1 = "v1";
    public const string V2 = "v2";
}
