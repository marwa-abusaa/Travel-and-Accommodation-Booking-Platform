using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IUserCommandService
{
    Task SignUpAsync(SignUpDto signUpDto);
}