using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Core.Entities;


namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CreateCityDto, City>();
        CreateMap<UpdateCityDto, City>();
        CreateMap<City, AdminCityResponseDto>();
        CreateMap<City, CityResponseDto>();
    }   
}
