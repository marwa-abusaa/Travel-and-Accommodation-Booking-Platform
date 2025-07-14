using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class HotelSearchDto
{
    public int HotelId { get; set; }
    public string Name { get; set; }
    public int StarRating { get; set; }
    public double PricePerNight { get; set; }
    public string Location { get; set; }
    public string BriefDescription { get; set; }
    public Image Thumbnail { get; set; }
}
