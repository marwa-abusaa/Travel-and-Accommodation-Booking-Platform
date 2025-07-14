using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Models;

public class VisitedHotelDto
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public Image Thumbnail { get; set; }
    public decimal TotalPrice { get; set; } 
}
