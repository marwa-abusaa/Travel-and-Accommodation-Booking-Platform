using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Review> AddReviewAsync(Review review);
    Task<Review?> GetReviewByIdAsync(int reviewId);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewByIdAsync(int reviewId);
    Task<double> GetHotelRatingAsync(int hotelId);
    Task<PaginatedResult<Review>> GetHotelReviewsAsync(int hotelId, PaginationMetadata pagination);
    Task<Review?> GetUserReviewForHotelAsync(int userId, int hotelId);
}
