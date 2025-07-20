using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Cities;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CityUpdateRequestDto, UpdateCityDto>();
    }
}
