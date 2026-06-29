using System.Text.Json.Serialization;

namespace KlaviyoTest.Klaviyo.Models;

public class KlaviyoMember
{
    [JsonPropertyName("data")]
    public KlaviyoMemberData Data { get; set; } = new();
}

public class KlaviyoMemberData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "profile";

    [JsonPropertyName("attributes")]
    public KlaviyoMemberAttributes Attributes { get; set; } = new();
}

public class KlaviyoMemberAttributes
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("birthday")]
    public string? Birthday { get; set; }

    [JsonPropertyName("location")]
    public KlaviyoLocation? Location { get; set; }

    [JsonPropertyName("properties")]
    public KlaviyoProperties Properties { get; set; } = new();
}

public class KlaviyoLocation
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class KlaviyoProperties
{
    [JsonPropertyName("loyalty_points")]
    public int LoyaltyPoints { get; set; }

    [JsonPropertyName("enrolled_at")]
    public string EnrolledAt { get; set; } = string.Empty;
}
