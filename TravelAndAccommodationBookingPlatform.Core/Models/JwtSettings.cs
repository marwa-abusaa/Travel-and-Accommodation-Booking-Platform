namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class JwtSettings
{
    public string Key => Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; }
}
