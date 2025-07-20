using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class ReviewQueryServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepoMock = new();
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<ILogger<ReviewQueryService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly ReviewQueryService _service;

    public ReviewQueryServiceTests()
    {
        _service = new ReviewQueryService(
            _reviewRepoMock.Object,
            _hotelRepoMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetHotelReviewsAsync_ShouldReturnPaginatedReviews_WhenHotelExists()
    {
        // Arrange
        int hotelId = 1;
        var pagination = new PaginationMetadata
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 10 
        };
        var hotel = new Hotel { HotelId = hotelId };
        var reviews = new List<Review> { new Review(), new Review() };
        var paginated = new PaginatedResult<Review>(reviews, pagination);
        var expectedDto = new List<ReviewResponseDto> { new(), new() };

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(hotelId)).ReturnsAsync(hotel);
        _reviewRepoMock.Setup(r => r.GetHotelReviewsAsync(hotelId, pagination)).ReturnsAsync(paginated);
        _mapperMock.Setup(m => m.Map<List<ReviewResponseDto>>(reviews)).Returns(expectedDto);

        // Act
        var result = await _service.GetHotelReviewsAsync(hotelId, pagination);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetHotelReviewsAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        int hotelId = 99;
        var pagination = new PaginationMetadata
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 10
        };

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(hotelId)).ReturnsAsync((Hotel)null);

        // Act
        Func<Task> act = async () => await _service.GetHotelReviewsAsync(hotelId, pagination);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Hotel with ID '{hotelId}' not found.");
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldReturnReview_WhenReviewExists()
    {
        // Arrange
        int reviewId = 5;
        var review = new Review { ReviewId = reviewId };
        var dto = new ReviewResponseDto { ReviewId = reviewId };

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(reviewId)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewResponseDto>(review)).Returns(dto);

        // Act
        var result = await _service.GetReviewByIdAsync(reviewId);

        // Assert
        result.Should().NotBeNull();
        result!.ReviewId.Should().Be(reviewId);
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
    {
        // Arrange
        int reviewId = 100;
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(reviewId)).ReturnsAsync((Review)null);

        // Act
        Func<Task> act = async () => await _service.GetReviewByIdAsync(reviewId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Review with ID '{reviewId}' not found.");
    }
}
