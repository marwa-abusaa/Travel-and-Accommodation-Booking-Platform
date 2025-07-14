using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IReviewCommandService
{
    Task<ReviewResponseDto> AddReviewAsync(CreateReviewDto createReviewDto);
    Task UpdateReviewAsync(UpdateReviewDto updateReviewDto);
    Task DeleteReviewByIdAsync(int reviewId);
}