namespace Mimo.AppStoreServerLibrary.Models;

/// <summary>
/// The age of the customer's account.
/// 0: Undeclared
/// 1: 0-3 days
/// 2: 3-10 days
/// 3: 10-30 days
/// 4: 30-90 days
/// 5: 90-180 days
/// 6: 180-365 days
/// 7: >365 days
/// </summary>
public enum AccountTenure
{
    Undeclared = 0,
    ZeroToThreeDays = 1,
    ThreeDaysToTenDays = 2,
    TenDaysToThirtyDays = 3,
    ThirtyDaysToNinetyDays = 4,
    NinetyDaysToOneHundredEightyDays = 5,
    OneHundredEightyDaysToThreeHundredSixtyFiveDays = 6,
    GreaterThanThreeHundredSixtyFiveDays = 7,
}

/// <summary>
/// The renewal status for an auto-renewable subscription.
/// </summary>
public enum AutoRenewStatus
{
    Off = 0,
    On = 1,
}

/// <summary>
/// The consumption status of an in-app purchase.
/// </summary>
public enum ConsumptionStatus
{
    Undeclared = 0,
    NotConsumed = 1,
    PartiallyConsumed = 2,
    FullyConsumed = 3,
}

/// <summary>
/// The delivery status of a consumable in-app purchase (deprecated v1 numeric version).
/// </summary>
public enum DeliveryStatusV1
{
    DeliveredAndWorkingProperly = 0,
    DidNotDeliverDueToQualityIssue = 1,
    DeliveredWrongItem = 2,
    DidNotDeliverDueToServerOutage = 3,
    DidNotDeliverDueToInGameCurrencyChange = 4,
    DidNotDeliverForOtherReason = 5,
}

/// <summary>
/// The reason a subscription expired.
/// </summary>
public enum ExpirationIntent
{
    CustomerCancelled = 1,
    BillingError = 2,
    CustomerDidNotConsentToPriceIncrease = 3,
    ProductNotAvailable = 4,
    Other = 5,
}

/// <summary>
/// A value that indicates the dollar amount of in-app purchases the customer has made, across all platforms.
/// </summary>
public enum LifetimeDollarsPurchased
{
    Undeclared = 0,
    ZeroDollars = 1,
    OneCentToFortyNineDollarsAndNinetyNineCents = 2,
    FiftyDollarsToNinetyNineDollarsAndNinetyNineCents = 3,
    OneHundredDollarsToFourHundredNinetyNineDollarsAndNinetyNineCents = 4,
    FiveHundredDollarsToNineHundredNinetyNineDollarsAndNinetyNineCents = 5,
    OneThousandDollarsToOneThousandNineHundredNinetyNineDollarsAndNinetyNineCents = 6,
    TwoThousandDollarsOrGreater = 7,
}

/// <summary>
/// A value that indicates the dollar amount of refunds the customer has received, across all platforms.
/// </summary>
public enum LifetimeDollarsRefunded
{
    Undeclared = 0,
    ZeroDollars = 1,
    OneCentToFortyNineDollarsAndNinetyNineCents = 2,
    FiftyDollarsToNinetyNineDollarsAndNinetyNineCents = 3,
    OneHundredDollarsToFourHundredNinetyNineDollarsAndNinetyNineCents = 4,
    FiveHundredDollarsToNineHundredNinetyNineDollarsAndNinetyNineCents = 5,
    OneThousandDollarsToOneThousandNineHundredNinetyNineDollarsAndNinetyNineCents = 6,
    TwoThousandDollarsOrGreater = 7,
}

/// <summary>
/// A value that represents the promotional offer type.
/// </summary>
public enum OfferType
{
    IntroductoryOffer = 1,
    PromotionalOffer = 2,
    OfferCode = 3,
    WinBackOffer = 4,
}

/// <summary>
/// The platform on which the customer consumed the in-app purchase.
/// </summary>
public enum Platform
{
    Undeclared = 0,
    Apple = 1,
    NonApple = 2,
}

/// <summary>
/// A value that indicates the amount of time that the customer used the app.
/// </summary>
public enum PlayTime
{
    Undeclared = 0,
    ZeroToFiveMinutes = 1,
    FiveToSixtyMinutes = 2,
    OneToSixHours = 3,
    SixHoursToTwentyFourHours = 4,
    OneDayToFourDays = 5,
    FourDaysToSixteenDays = 6,
    OverSixteenDays = 7,
}

/// <summary>
/// The status that indicates whether an auto-renewable subscription is subject to a price increase.
/// </summary>
public enum PriceIncreaseStatus
{
    CustomerHasNotResponded = 0,
    CustomerConsentedOrWasNotifiedWithoutNeedingConsent = 1,
}

/// <summary>
/// A value that indicates your preferred outcome for the refund request (deprecated v1 numeric version).
/// </summary>
public enum RefundPreferenceV1
{
    Undeclared = 0,
    PreferGrant = 1,
    PreferDecline = 2,
    NoPreference = 3,
}

/// <summary>
/// The reason for a refund or revocation.
/// </summary>
public enum RevocationReason
{
    RefundedForOtherReason = 0,
    RefundedDueToIssue = 1,
}

/// <summary>
/// The status of the customer's account.
/// </summary>
public enum UserStatus
{
    Undeclared = 0,
    Active = 1,
    Suspended = 2,
    Terminated = 3,
    LimitedAccess = 4,
}

/// <summary>
/// Error codes returned by the App Store Server API.
/// </summary>
public enum ApiErrorCode : long
{
    GeneralBadRequest = 4000000,
    InvalidAppIdentifier = 4000002,
    InvalidRequestRevision = 4000005,
    InvalidTransactionId = 4000006,
    InvalidOriginalTransactionId = 4000008,
    InvalidExtendByDays = 4000009,
    InvalidExtendReasonCode = 4000010,
    InvalidRequestIdentifier = 4000011,
    StartDateTooFarInPast = 4000012,
    StartDateAfterEndDate = 4000013,
    InvalidPaginationToken = 4000014,
    InvalidStartDate = 4000015,
    InvalidEndDate = 4000016,
    PaginationTokenExpired = 4000017,
    InvalidNotificationType = 4000018,
    MultipleFiltersSupplied = 4000019,
    InvalidTestNotificationToken = 4000020,
    InvalidSort = 4000021,
    InvalidProductType = 4000022,
    InvalidProductId = 4000023,
    InvalidSubscriptionGroupIdentifier = 4000024,
    InvalidExcludeRevoked = 4000025,
    InvalidInAppOwnershipType = 4000026,
    InvalidEmptyStorefrontCountryCodeList = 4000027,
    InvalidStorefrontCountryCode = 4000028,
    InvalidRevoked = 4000030,
    InvalidStatus = 4000031,
    InvalidAccountTenure = 4000032,
    InvalidAppAccountToken = 4000033,
    InvalidConsumptionStatus = 4000034,
    InvalidCustomerConsented = 4000035,
    InvalidDeliveryStatus = 4000036,
    InvalidLifetimeDollarsPurchased = 4000037,
    InvalidLifetimeDollarsRefunded = 4000038,
    InvalidPlatform = 4000039,
    InvalidPlayTime = 4000040,
    InvalidSampleContentProvided = 4000041,
    InvalidUserStatus = 4000042,
    InvalidTransactionNotConsumable = 4000043,
    InvalidTransactionTypeNotSupported = 4000047,
    AppTransactionIdNotSupportedError = 4000048,
    InvalidImage = 4000161,
    HeaderTooLong = 4000162,
    BodyTooLong = 4000163,
    InvalidLocale = 4000164,
    AltTextTooLong = 4000175,
    InvalidAppAccountTokenUuidError = 4000183,
    FamilyTransactionNotSupportedError = 4000185,
    TransactionIdIsNotOriginalTransactionIdError = 4000187,
    SubscriptionExtensionIneligible = 4030004,
    SubscriptionMaxExtension = 4030005,
    FamilySharedSubscriptionExtensionIneligible = 4030007,
    MaximumNumberOfImagesReached = 4030014,
    MaximumNumberOfMessagesReached = 4030016,
    MessageNotApproved = 4030017,
    ImageNotApproved = 4030018,
    ImageInUse = 4030019,
    AccountNotFound = 4040001,
    AccountNotFoundRetryable = 4040002,
    AppNotFound = 4040003,
    AppNotFoundRetryable = 4040004,
    OriginalTransactionIdNotFound = 4040005,
    OriginalTransactionIdNotFoundRetryable = 4040006,
    ServerNotificationUrlNotFound = 4040007,
    TestNotificationNotFound = 4040008,
    StatusRequestNotFound = 4040009,
    TransactionIdNotFound = 4040010,
    ImageNotFound = 4040014,
    MessageNotFound = 4040015,
    AppTransactionDoesNotExistError = 4040019,
    ImageAlreadyExists = 4090000,
    MessageAlreadyExists = 4090001,
    RateLimitExceeded = 4290000,
    GeneralInternal = 5000000,
    GeneralInternalRetryable = 5000001,
}
