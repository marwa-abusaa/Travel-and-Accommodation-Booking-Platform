namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;

public class CreateDiscountDto
{
    public int RoomId { get; set; }
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
