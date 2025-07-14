using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;

namespace TravelAndAccommodationBookingPlatform.Application.SieveConfigurations;

public class AdminCitySieveConfiguration : ISieveConfiguration
{
    public void Configure(SievePropertyMapper mapper)
    {
        mapper.Property<AdminCityResponseDto>(x => x.Name).CanFilter().CanSort();
        mapper.Property<AdminCityResponseDto>(x => x.Country).CanFilter().CanSort();
        mapper.Property<AdminCityResponseDto>(x => x.PostOffice).CanFilter().CanSort();
        mapper.Property<AdminCityResponseDto>(x => x.HotelsCount).CanFilter().CanSort();
        mapper.Property<AdminCityResponseDto>(x => x.CreatedAt).CanSort();
        mapper.Property<AdminCityResponseDto>(x => x.UpdatedAt).CanSort();
    }
}
