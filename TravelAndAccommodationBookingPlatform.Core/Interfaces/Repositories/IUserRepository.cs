using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> IsEmailExistsAsync(string email);
}
