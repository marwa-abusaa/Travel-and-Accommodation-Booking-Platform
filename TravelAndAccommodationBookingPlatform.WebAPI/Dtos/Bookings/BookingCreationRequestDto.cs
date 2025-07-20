using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Bookings;

public class BookingCreationRequestDto
{
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public List<int> RoomIds { get; set; }
    public string? Remarks { get; set; }
    public PaymentType PaymentType { get; set; }
}
