using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<RoomImageRequestDto, CreateImageDto>();
        CreateMap<RoomImageRequestDto, UpdateImageDto>();
        CreateMap<HotelImageRequestDto, CreateImageDto>();
        CreateMap<HotelImageRequestDto, UpdateImageDto>();
    }
}
