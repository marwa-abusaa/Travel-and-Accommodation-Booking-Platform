namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Image
{
    public int ImageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int HotelId { get; set; }
    public int RoomId { get; set; }
    public Hotel Hotel { get; set; }
    public Room? Room { get; set; }
}
