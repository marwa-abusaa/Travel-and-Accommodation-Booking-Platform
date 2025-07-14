namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class AdminHotelResponseDto
{
    public int HotelId { get; set; }
    public string Name { get; set; }
    public string OwnerName { get; set; }
    public int RoomCount { get; set; }
    public int StarRating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
