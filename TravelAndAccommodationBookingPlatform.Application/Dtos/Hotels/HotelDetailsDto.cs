using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class HotelDetailsDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public int StarRating { get; set; }
    public double ReviewRating { get; set; }
    public string BriefDescription { get; set; }
    public string FullDescription { get; set; }
    public string CityName { get; set; }
    public string OwnerName { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    public IEnumerable<ImageResponseDto> Images { get; set; } = new List<ImageResponseDto>();
}
