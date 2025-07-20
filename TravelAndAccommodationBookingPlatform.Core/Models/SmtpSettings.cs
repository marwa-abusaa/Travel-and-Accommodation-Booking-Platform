namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class SmtpSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password => Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
}
