using Sieve.Attributes;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class HotelSearchDto
{
    public int HotelId { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public int StarRating { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public decimal PricePerNight { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public string Location { get; set; }

    public string BriefDescription { get; set; }
    public ImageResponseDto Thumbnail { get; set; }
}
