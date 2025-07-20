using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;     
    }
    public async Task<Review> AddReviewAsync(Review review)
    {
        var newReview = await _context.Reviews.AddAsync(review);
        return newReview.Entity;
    }

    public async Task DeleteReviewByIdAsync(int reviewId)
    {
        var review = await GetReviewByIdAsync(reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
        }
    }

    public async Task<double> GetHotelRatingAsync(int hotelId)
    {
        var hotelRatings = await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .Select(r => r.Rate)
            .ToListAsync();

        if (!hotelRatings.Any())
            return 0;

        return hotelRatings.Average();
    }

    public async Task<PaginatedResult<Review>> GetHotelReviewsAsync(int hotelId, PaginationMetadata pagination)
    {
        pagination.TotalCount = await _context.Reviews.CountAsync();

        if (pagination.PageNumber > pagination.TotalPages && pagination.TotalPages != 0)
            pagination.PageNumber = pagination.TotalPages;

        var skip = (pagination.PageNumber - 1) * pagination.PageSize;

        var query = _context.Reviews.AsQueryable();

        var items = await query
            .Skip(skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResult<Review>(items, pagination);
    }

    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await _context.Reviews.FindAsync(reviewId);
    }


    public async Task UpdateReviewAsync(Review review)
    {
        if (await GetReviewByIdAsync(review.ReviewId) != null)
        {
            _context.Reviews.Update(review);
        }
    }
}
