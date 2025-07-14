using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;

public interface IBookingHtmlBuilder
{
    public string BuildConfirmationHtml(Booking booking);
}
