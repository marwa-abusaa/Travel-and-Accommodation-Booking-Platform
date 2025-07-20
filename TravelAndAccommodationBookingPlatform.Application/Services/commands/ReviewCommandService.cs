using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class ReviewCommandService : IReviewCommandService
{
    private readonly IUserRepository _userRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReviewRepository _reviewRepository;
    private readonly ILogger<ReviewCommandService> _logger;
    private readonly IMapper _mapper;

    public ReviewCommandService(
        IUserRepository userRepository,
        IHotelRepository hotelRepository,
        IUnitOfWork unitOfWork,
        IReviewRepository reviewRepository, 
        ILogger<ReviewCommandService> logger,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _reviewRepository = reviewRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ReviewResponseDto> AddReviewAsync(CreateReviewDto createReviewDto)
    {
        var user = await _userRepository.GetUserByIdAsync(createReviewDto.UserId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", createReviewDto.UserId);
            throw new NotFoundException($"User with ID '{createReviewDto.UserId}' not found.");
        }

        var hotel = await _hotelRepository.GetHotelByIdAsync(createReviewDto.HotelId);
        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", createReviewDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{createReviewDto.HotelId}' not found.");
        }

        var review = _mapper.Map<Review>(createReviewDto);
        var addedReview = await _reviewRepository.AddReviewAsync(review);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Review with ID {ReviewId} added successfully.", addedReview.ReviewId);

        return _mapper.Map<ReviewResponseDto>(addedReview);
    }

    public async Task DeleteReviewByIdAsync(int reviewId)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
        if (review is null)
        {
            _logger.LogWarning("Review with ID {ReviewId} not found.", reviewId);
            throw new NotFoundException($"Review with ID '{reviewId}' not found.");
        }

        await _reviewRepository.DeleteReviewByIdAsync(reviewId);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Review with ID {ReviewId} deleted successfully.", reviewId);
    }

    public async Task UpdateReviewAsync(UpdateReviewDto updateReviewDto)
    {
        var existingReview = await _reviewRepository.GetReviewByIdAsync(updateReviewDto.ReviewId);
        if (existingReview is null)
        {
            _logger.LogWarning("Review with ID {ReviewId} not found.", updateReviewDto.ReviewId);
            throw new NotFoundException($"Review with ID '{updateReviewDto.ReviewId}' not found.");
        }

        var user = await _userRepository.GetUserByIdAsync(updateReviewDto.UserId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", updateReviewDto.UserId);
            throw new NotFoundException($"User with ID '{updateReviewDto.UserId}' not found.");
        }

        var hotel = await _hotelRepository.GetHotelByIdAsync(updateReviewDto.HotelId);
        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", updateReviewDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{updateReviewDto.HotelId}' not found.");
        }

        _mapper.Map(updateReviewDto, existingReview);
        await _reviewRepository.UpdateReviewAsync(existingReview);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Review with ID {ReviewId} updated successfully.", updateReviewDto.ReviewId);
    }
}
