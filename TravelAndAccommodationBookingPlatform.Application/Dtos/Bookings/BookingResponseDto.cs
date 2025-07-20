using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public decimal TotalPriceBeforeDiscount { get; set; }
    public decimal TotalPriceAfterDiscount { get; set; }
    public string Remarks { get; set; }
    public List<RoomResponseDto> Rooms { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public DateOnly BookingDate { get; init; }
}
