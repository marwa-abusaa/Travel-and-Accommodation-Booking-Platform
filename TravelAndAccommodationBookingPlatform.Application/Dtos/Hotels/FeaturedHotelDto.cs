using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class FeaturedHotelDto
{
    public int HotelId { get; set; }
    public string HotelName { get; set; }
    public string Location { get; set; }
    public Image Thumbnail { get; set; }
    public int StarRating { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
}
