namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Discount
{
    public int DiscountId { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
