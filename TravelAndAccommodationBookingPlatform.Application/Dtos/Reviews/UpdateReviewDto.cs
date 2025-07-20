namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;

public class UpdateReviewDto
{
    public int ReviewId { get; set; }
    public string Comment { get; set; }
    public int Rate { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
}
