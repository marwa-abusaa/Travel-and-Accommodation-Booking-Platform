using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;
using TravelAndAccommodationBookingPlatform.Core.Entities;


namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class CityMapper : Profile
{
    public CityMapper()
    {
        CreateMap<CreateCityDto, City>();
        CreateMap<UpdateCityDto, City>();
        CreateMap<City, AdminCityResponseDto>();
        CreateMap<City, CityResponseDto>();
    }   
}
