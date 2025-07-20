using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Users;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Authentication;
using TravelAndAccommodationBookingPlatform.WebAPI.Shared;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpRequestDto, SignUpDto>()
           .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => UserRoles.Guest));

        CreateMap<LogInRequestDto, LogInDto>();
    }
}
