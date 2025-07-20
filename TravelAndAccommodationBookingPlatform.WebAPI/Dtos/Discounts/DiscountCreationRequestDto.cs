namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Discounts;

public class DiscountCreationRequestDto
{
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
