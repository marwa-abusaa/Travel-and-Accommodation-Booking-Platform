using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IOwnerRepository
{
    public Task<Owner> AddOwnerAsync(Owner owner);
    public Task UpdateOwnerAsync(Owner owner);
    public Task<Owner?> GetOwnerByIdAsync(int ownerId);
    public Task<Owner?> GetOwnerByHotelIdAsync(int hotelId);
}
