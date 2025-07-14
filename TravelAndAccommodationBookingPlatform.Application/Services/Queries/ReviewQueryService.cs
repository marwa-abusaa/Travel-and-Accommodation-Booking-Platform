using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class ReviewQueryService : IReviewQueryService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly ILogger<ReviewQueryService> _logger;
    private readonly IMapper _mapper;

    public ReviewQueryService(
        IReviewRepository reviewRepository, 
        IHotelRepository hotelRepository, 
        ILogger<ReviewQueryService> logger,
        IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ReviewResponseDto>> GetHotelReviewsAsync(int hotelId, PaginationMetadata pagination)
    {
        _logger.LogInformation("Fetching reviews for hotel ID {HotelId}.", hotelId);

        var hotel = await _hotelRepository.GetHotelByIdAsync(hotelId);
        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", hotelId);
            throw new NotFoundException($"Hotel with ID '{hotelId}' not found."); ;
        }

        var reviews = await _reviewRepository.GetHotelReviewsAsync(hotelId, pagination);

        _logger.LogInformation("Fetched {Count} reviews for hotel ID {HotelId}.", reviews.Items.Count, hotelId);

        return _mapper.Map<PaginatedResult<ReviewResponseDto>>(reviews);
    }

    public async Task<ReviewResponseDto?> GetReviewByIdAsync(int reviewId)
    {
        _logger.LogInformation("Fetching review with ID {ReviewId}.", reviewId);

        var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
        if (review is null)
        {
            _logger.LogWarning("Review with ID {ReviewId} not found.", reviewId);
            throw new NotFoundException($"Review with ID '{reviewId}' not found.");
        }

        _logger.LogInformation("Review with ID {ReviewId} fetched successfully.", reviewId);

        return _mapper.Map<ReviewResponseDto>(review);
    }
}
