using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IUserQueryService
{
    Task<TokenResponseDto> LogInAsync(LogInDto logInDto);
}
