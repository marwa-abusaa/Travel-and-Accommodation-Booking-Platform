using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class ImageMapper : Profile
{
    public ImageMapper()
    {
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();
        CreateMap<Image, ImageResponseDto>();
    }
}
