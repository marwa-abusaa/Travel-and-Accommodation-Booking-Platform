using Sieve.Attributes;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;

public class AdminCityResponseDto
{
    public int CityId { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Country { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string PostOffice { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int HotelsCount { get; set; }

    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; set; }

    [Sieve(CanSort = true)]
    public DateTime? UpdatedAt { get; set; }
}
