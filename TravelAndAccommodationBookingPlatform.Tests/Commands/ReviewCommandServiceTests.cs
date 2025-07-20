using Moq;
using FluentAssertions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class ReviewCommandServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<IReviewRepository> _reviewRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<ReviewCommandService>> _loggerMock = new();
    private readonly ReviewCommandService _service;

    public ReviewCommandServiceTests()
    {
        _service = new ReviewCommandService(
            _userRepoMock.Object,
            _hotelRepoMock.Object,
            _unitOfWorkMock.Object,
            _reviewRepoMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task AddReviewAsync_ShouldAddReview_WhenUserAndHotelExist()
    {
        // Arrange
        var dto = new CreateReviewDto { UserId = 1, HotelId = 1, Comment = "Nice", Rate = 5 };
        var user = new User { UserId = 1 };
        var hotel = new Hotel { HotelId = 1 };
        var review = new Review { ReviewId = 10 };
        var addedReview = new Review { ReviewId = 10 };
        var reviewDto = new ReviewResponseDto { ReviewId = 10 };

        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId)).ReturnsAsync(user);
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.HotelId)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<Review>(dto)).Returns(review);
        _reviewRepoMock.Setup(r => r.AddReviewAsync(review)).ReturnsAsync(addedReview);
        _mapperMock.Setup(m => m.Map<ReviewResponseDto>(addedReview)).Returns(reviewDto);

        // Act
        var result = await _service.AddReviewAsync(dto);

        // Assert
        result.Should().BeEquivalentTo(reviewDto);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task AddReviewAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        var dto = new CreateReviewDto { UserId = 1, HotelId = 123 };
        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId)).ReturnsAsync(new User());
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.HotelId)).ReturnsAsync((Hotel?)null);

        // Act
        var act = async () => await _service.AddReviewAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Hotel with ID '{dto.HotelId}' not found.");
    }


    [Fact]
    public async Task DeleteReviewByIdAsync_ShouldDelete_WhenReviewExists()
    {
        // Arrange
        int reviewId = 5;
        var review = new Review { ReviewId = reviewId };
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(reviewId)).ReturnsAsync(review);

        // Act
        await _service.DeleteReviewByIdAsync(reviewId);

        // Assert
        _reviewRepoMock.Verify(r => r.DeleteReviewByIdAsync(reviewId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteReviewByIdAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
    {
        // Arrange
        int reviewId = 100;
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(reviewId)).ReturnsAsync((Review?)null);

        // Act
        var act = async () => await _service.DeleteReviewByIdAsync(reviewId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Review with ID '{reviewId}' not found.");
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldUpdate_WhenAllEntitiesExist()
    {
        // Arrange
        var dto = new UpdateReviewDto { ReviewId = 1, UserId = 2, HotelId = 3, Comment = "Updated", Rate = 4 };
        var review = new Review { ReviewId = 1 };
        var user = new User { UserId = 2 };
        var hotel = new Hotel { HotelId = 3 };

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.ReviewId)).ReturnsAsync(review);
        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId)).ReturnsAsync(user);
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.HotelId)).ReturnsAsync(hotel);

        // Act
        await _service.UpdateReviewAsync(dto);

        // Assert
        _mapperMock.Verify(m => m.Map(dto, review), Times.Once);
        _reviewRepoMock.Verify(r => r.UpdateReviewAsync(review), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldThrowNotFoundException_WhenReviewNotFound()
    {
        // Arrange
        var dto = new UpdateReviewDto { ReviewId = 1 };
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.ReviewId)).ReturnsAsync((Review?)null);

        // Act
        var act = async () => await _service.UpdateReviewAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Review with ID '{dto.ReviewId}' not found.");
    }

}
