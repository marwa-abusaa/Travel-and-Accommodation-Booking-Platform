using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string name);
}
