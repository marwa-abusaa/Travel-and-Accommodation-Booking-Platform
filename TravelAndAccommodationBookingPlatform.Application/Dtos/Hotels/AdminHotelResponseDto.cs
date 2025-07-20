using Sieve.Attributes;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

public class AdminHotelResponseDto
{
    public int HotelId { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public int StarRating { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public string OwnerName { get; set; }


    [Sieve(CanFilter = true, CanSort = true)]
    public int RoomCount { get; set; }


    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; set; }


    [Sieve(CanSort = true)]
    public DateTime? UpdatedAt { get; set; }
}
