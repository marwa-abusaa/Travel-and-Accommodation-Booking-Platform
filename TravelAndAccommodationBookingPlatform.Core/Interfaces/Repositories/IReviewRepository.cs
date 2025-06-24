using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    public Task<Review> AddReviewAsync(Review review);
    public Task<Review?> GetReviewByIdAsync(int reviewId, int hotelId);
    public Task UpdateReviewAsync(Review review);
    public Task DeleteReviewAsync(int reviewId);
    Task<int> GetHotelRatingAsync(int hotelId);
    public Task<IEnumerable<Review>> GetHotelReviewsAsync(int hotelId);
    public Task<IEnumerable<Review>> GetUserReviewsAsync(int userId, int hotelId);
}
