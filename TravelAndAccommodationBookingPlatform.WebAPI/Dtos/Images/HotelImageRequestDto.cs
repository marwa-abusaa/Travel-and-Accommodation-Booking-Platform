using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Images;

public class HotelImageRequestDto
{
    public string Name { get; set; }
    public string Path { get; set; }
    public ImageType Type { get; set; }
}
