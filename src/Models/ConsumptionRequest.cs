using System.Text.Json.Serialization;

namespace Mimo.AppStoreServerLibrary.Models;

/// <summary>
/// The request body containing consumption information (v2, string-based fields).
/// </summary>
public record ConsumptionRequestV2
{
    /// <summary>
    /// Whether the customer consented to the consumption information being shared.
    /// </summary>
    [JsonPropertyName("customerConsented")]
    public required bool CustomerConsented { get; set; }

    /// <summary>
    /// A value between 0 and 100 that represents the percentage of the in-app purchase content the customer consumed.
    /// </summary>
    [JsonPropertyName("consumptionPercentage")]
    public int? ConsumptionPercentage { get; set; }

    /// <summary>
    /// The delivery status of the consumable in-app purchase.
    /// </summary>
    [JsonPropertyName("deliveryStatus")]
    public required string DeliveryStatus { get; set; }

    /// <summary>
    /// Your preference for handling the refund.
    /// </summary>
    [JsonPropertyName("refundPreference")]
    public string? RefundPreference { get; set; }

    /// <summary>
    /// Whether sample content was provided to the user.
    /// </summary>
    [JsonPropertyName("sampleContentProvided")]
    public required bool SampleContentProvided { get; set; }
}

/// <summary>
/// Response for a consumption request notification, used by Apple to determine whether to grant the user the refund or decline it.
/// </summary>
public record ConsumptionRequest
{
    /// <summary>
    /// Account tenure value based on user's account age:
    /// 0: Undeclared
    /// 1: 0-3 days
    /// 2: 3-10 days
    /// 3: 10-30 days
    /// 4: 30-90 days
    /// 5: 90-180 days
    /// 6: 180-365 days
    /// 7: >365 days
    /// </summary>
    [JsonPropertyName("accountTenure")]
    public required AccountTenure AccountTenure { get; set; }

    /// <summary>
    /// The app account token associated with the user's subscription
    /// </summary>
    [JsonPropertyName("appAccountToken")]
    public required string AppAccountToken { get; set; }

    /// <summary>
    /// The consumption status of the in-app purchase:
    /// 0: Undeclared
    /// 1: Not consumed
    /// 2: Partially consumed
    /// 3: Fully consumed
    /// </summary>
    [JsonPropertyName("consumptionStatus")]
    public required ConsumptionStatus ConsumptionStatus { get; set; }

    /// <summary>
    /// Whether the customer consented to the consumption information being shared
    /// </summary>
    [JsonPropertyName("customerConsented")]
    public required bool CustomerConsented { get; set; }

    /// <summary>
    /// The delivery status of the consumable in-app purchase:
    /// 0: Delivered and working properly
    /// 1: Not delivered due to quality issue
    /// 2: Wrong item delivered
    /// 3: Not delivered due to server outage
    /// 4: Not delivered due to in-game currency change
    /// 5: Not delivered for other reasons
    /// </summary>
    [JsonPropertyName("deliveryStatus")]
    public required DeliveryStatusV1 DeliveryStatus { get; set; }

    /// <summary>
    /// The dollar amount of in-app purchases made across all platforms:
    /// 0: Undeclared
    /// 1: $0
    /// 2: $0.01-$49.99
    /// 3: $50-$99.99
    /// 4: $100-$499.99
    /// 5: $500-$999.99
    /// 6: $1000-$1999.99
    /// 7: Over $2000
    /// </summary>
    [JsonPropertyName("lifetimeDollarsPurchased")]
    public required LifetimeDollarsPurchased LifetimeDollarsPurchased { get; set; }

    /// <summary>
    /// The dollar amount of refunds the customer has received in your app, since purchasing the app, across all platforms:
    /// 0: Undeclared
    /// 1: $0
    /// 2: $0.01-$49.99
    /// 3: $50-$99.99
    /// 4: $100-$499.99
    /// 5: $500-$999.99
    /// 6: $1000-$1999.99
    /// 7: Over $2000
    /// </summary>
    [JsonPropertyName("lifetimeDollarsRefunded")]
    public required LifetimeDollarsRefunded LifetimeDollarsRefunded { get; set; }

    /// <summary>
    /// The platform the app is running on:
    /// 0: Undeclared
    /// 1: Apple platform
    /// 2: Non-Apple platform
    /// </summary>
    [JsonPropertyName("platform")]
    public required Platform Platform { get; set; }

    /// <summary>
    /// The user's engagement time with the app:
    /// 0: Undeclared
    /// 1: 0-5 minutes
    /// 2: 5-60 minutes
    /// 3: 1-6 hours
    /// 4: 6-24 hours
    /// 5: 1-4 days
    /// 6: 4-16 days
    /// 7: Over 16 days
    /// </summary>
    [JsonPropertyName("playTime")]
    public required PlayTime PlayTime { get; set; }

    /// <summary>
    /// Your preference for handling the refund:
    /// 0: Undeclared
    /// 1: Prefer to grant refund
    /// 2: Prefer to decline refund
    /// 3: No preference
    /// </summary>
    [JsonPropertyName("refundPreference")]
    public required RefundPreferenceV1 RefundPreference { get; set; }

    /// <summary>
    /// Whether sample content was provided to the user
    /// </summary>
    [JsonPropertyName("sampleContentProvided")]
    public required bool SampleContentProvided { get; set; }

    /// <summary>
    /// The status of the user's account:
    /// 0: Undeclared
    /// 1: Active
    /// 2: Suspended
    /// 3: Terminated
    /// 4: Limited access
    /// </summary>
    [JsonPropertyName("userStatus")]
    public required UserStatus UserStatus { get; set; }
}
