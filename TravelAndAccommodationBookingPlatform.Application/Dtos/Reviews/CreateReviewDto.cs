namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;

public class CreateReviewDto
{
    public string Comment { get; set; }
    public int Rate { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
}
