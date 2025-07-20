namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;

public class ReviewResponseDto
{
    public int ReviewId { get; set; }
    public string Comment { get; set; }
    public int Rate { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
