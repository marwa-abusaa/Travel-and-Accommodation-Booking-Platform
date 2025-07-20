using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

public class CreateImageDto
{
    public string Name { get; set; }
    public string Path { get; set; }
    public ImageType Type { get; set; }
    public int HotelId { get; set; }
    public int? RoomId { get; set; }
}
