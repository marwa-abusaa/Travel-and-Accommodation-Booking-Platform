using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<SignUpDto, User>();
        CreateMap<JwtToken, TokenResponseDto>();
    }
}
