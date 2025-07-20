using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

public class UpdateRoomDto
{
    public int RoomId { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int HotelId { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public RoomClass RoomClass { get; set; }
}
