using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IOwnerRepository
{
    public Task<Owner> AddOwnerAsync(Owner owner);
    public Task<IEnumerable<Owner>> GetHotelOwnersAsync(int hotelId);
    public Task UpdateOwnerAsync(Owner owner);
    public Task<Owner?> GetOwnerByIdAsync(int ownerId);
}
