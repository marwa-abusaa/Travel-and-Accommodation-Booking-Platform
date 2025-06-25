using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public decimal TotalPriceBeforeDiscount { get; set; }
    public decimal TotalPriceAfterDiscount { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public PaymentType PaymentType { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public DateOnly BookingDate { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
