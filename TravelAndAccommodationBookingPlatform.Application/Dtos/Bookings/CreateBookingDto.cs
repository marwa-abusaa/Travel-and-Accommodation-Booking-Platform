using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;

public class CreateBookingDto
{
    public int UserId { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public List<int> RoomIds { get; set; }
    public string? Remarks { get; set; }
    public PaymentType PaymentType { get; set; }
}
