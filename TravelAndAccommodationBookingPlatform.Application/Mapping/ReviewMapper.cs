using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Mapping;

public class ReviewMapper : Profile
{
    public ReviewMapper()
    {
        CreateMap<CreateReviewDto, Review>();
        CreateMap<UpdateReviewDto, Review>();
        CreateMap<Review, ReviewResponseDto>();
    }
}
