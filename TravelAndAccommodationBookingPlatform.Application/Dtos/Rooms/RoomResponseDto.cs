using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

public class RoomResponseDto
{
    public int RoomId { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public RoomClass RoomClass { get; set; }
}
