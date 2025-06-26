using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IOwnerRepository
{
    Task<Owner> AddOwnerAsync(Owner owner);
    Task UpdateOwnerAsync(Owner owner);
    Task<Owner?> GetOwnerByIdAsync(int ownerId);
    Task<Owner?> GetOwnerByHotelIdAsync(int hotelId);
}   
