using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IUserRepository
{
    public Task AddUserAsync(User user);
    public Task<User?> GetUserByIdAsync(int id);
    public Task<bool> IsEmailExistsAsync(string email);
}
