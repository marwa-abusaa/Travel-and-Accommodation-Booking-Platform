using Sieve.Attributes;
using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

public class AdminRoomResponseDto
{
    public int RoomId { get; set; }

    [Sieve(CanFilter = true)]
    public bool IsAvailable { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int AdultCapacity { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int ChildrenCapacity { get; set; }

    [Sieve(CanFilter = true)]
    public RoomClass RoomClass { get; set; }

    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; set; }

    [Sieve(CanSort = true)]
    public DateTime? UpdatedAt { get; set; }
}
