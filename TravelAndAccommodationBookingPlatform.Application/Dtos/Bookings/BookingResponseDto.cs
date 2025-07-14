using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public List<Room> Rooms { get; set; }
    public decimal TotalPriceBeforeDiscount { get; set; }
    public decimal TotalPriceAfterDiscount { get; set; }
    public string? Remarks { get; set; }
    public PaymentType PaymentType { get; set; }
    public DateOnly CheckInDate { get; init; }
    public DateOnly CheckOutDate { get; init; }
    public DateOnly BookingDate { get; init; }
}
