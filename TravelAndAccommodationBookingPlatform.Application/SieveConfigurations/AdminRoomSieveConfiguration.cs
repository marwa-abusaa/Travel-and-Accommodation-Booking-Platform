using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

namespace TravelAndAccommodationBookingPlatform.Application.SieveConfigurations;

public class AdminRoomSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<AdminRoomResponseDto>(r => r.IsAvailable).CanFilter();
        mapper.Property<AdminRoomResponseDto>(r => r.AdultCapacity).CanFilter().CanSort();
        mapper.Property<AdminRoomResponseDto>(r => r.ChildrenCapacity).CanFilter().CanSort();
        mapper.Property<AdminRoomResponseDto>(r => r.RoomClass).CanFilter().CanSort();
        mapper.Property<AdminRoomResponseDto>(r => r.CreatedAt).CanSort();
        mapper.Property<AdminRoomResponseDto>(r => r.UpdatedAt).CanSort();
    }
}
