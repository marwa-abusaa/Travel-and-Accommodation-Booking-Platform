using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.SieveConfigurations;

public class HotelSieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<Hotel>(h => h.Name)
              .CanFilter()
              .CanSort();

        mapper.Property<Hotel>(h => h.StarRating)
              .CanFilter()
              .CanSort();

        mapper.Property<Hotel>(h => h.Rooms.First().PricePerNight)
              .HasName("MinRoomPrice")
              .CanSort();
    }
}
