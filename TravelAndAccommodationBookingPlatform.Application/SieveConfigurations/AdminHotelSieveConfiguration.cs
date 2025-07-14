using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

namespace TravelAndAccommodationBookingPlatform.Application.SieveConfigurations;

public class AdminHotelSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<AdminHotelResponseDto>(x => x.Name).CanFilter().CanSort();
        mapper.Property<AdminHotelResponseDto>(x => x.StarRating).CanFilter().CanSort();
        mapper.Property<AdminHotelResponseDto>(x => x.OwnerName).CanFilter().CanSort();
        mapper.Property<AdminHotelResponseDto>(x => x.RoomCount).CanFilter().CanSort();
        mapper.Property<AdminHotelResponseDto>(x => x.CreatedAt).CanSort();
        mapper.Property<AdminHotelResponseDto>(x => x.UpdatedAt).CanSort();
    }
}
