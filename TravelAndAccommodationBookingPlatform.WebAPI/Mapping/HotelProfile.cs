using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Hotels;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<HotelUpdateRequestDto, UpdateHotelDto>();
    }
}
