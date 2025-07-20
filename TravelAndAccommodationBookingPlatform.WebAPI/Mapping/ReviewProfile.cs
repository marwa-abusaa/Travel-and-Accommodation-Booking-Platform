using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Reviews;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<ReviewCreationRequestDto, CreateReviewDto>();
    }
}
