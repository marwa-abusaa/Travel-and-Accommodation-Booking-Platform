using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> AuthenticateUserAsync(string email, string password);
    public Task AddUserAsync(User user);
    public Task<User?> GetUserByIdAsync(int id);
    public Task<bool> IsEmailExistsAsync(string email);
}
