using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class RecentHotelDto
{
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string CityName { get; set; }
    public int StarRating { get; set; }
    public ImageResponseDto? Thumbnail { get; set; }
    public decimal TotalPrice { get; set; }
}
