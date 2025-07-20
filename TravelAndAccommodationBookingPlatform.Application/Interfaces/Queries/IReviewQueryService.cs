using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IReviewQueryService
{
    Task<ReviewResponseDto?> GetReviewByIdAsync(int reviewId);
    Task<PaginatedResult<ReviewResponseDto>> GetHotelReviewsAsync(int hotelId, PaginationMetadata pagination);
}
