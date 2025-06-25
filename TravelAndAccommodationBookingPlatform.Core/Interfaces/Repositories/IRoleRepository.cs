using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    public Task<Role?> GetRoleByNameAsync(string name);
}
