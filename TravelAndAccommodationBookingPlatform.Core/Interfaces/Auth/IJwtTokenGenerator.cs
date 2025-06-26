using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Auth;

public interface IJwtTokenGenerator
{
    JwtToken GenerateToken(User inputUser);
}