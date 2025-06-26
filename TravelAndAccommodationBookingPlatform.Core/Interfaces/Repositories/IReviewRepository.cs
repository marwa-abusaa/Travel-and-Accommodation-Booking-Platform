using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    public Task<Review> AddReviewAsync(Review review);
    public Task<Review?> GetReviewByIdAsync(int reviewId);
    public Task UpdateReviewAsync(Review review);
    public Task DeleteReviewAsync(int reviewId);
    Task<double> GetHotelRatingAsync(int hotelId);
    public Task<PaginatedResult<Review>> GetHotelReviewsAsync(int hotelId, PaginationMetadata pagination);
    public Task<Review> GetUserReviewAsync(int userId, int hotelId);
}
