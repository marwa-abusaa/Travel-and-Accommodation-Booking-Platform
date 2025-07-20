using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();
        CreateMap<Image, ImageResponseDto>();
    }
}
