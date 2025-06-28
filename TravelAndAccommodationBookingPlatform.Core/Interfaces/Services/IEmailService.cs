using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendPaymentConfirmationEmailAsync(EmailRequest emailRequest);
}
