using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;
using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Reviews;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Models;


namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class HotelQueryServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IOwnerRepository> _ownerRepoMock;
    private readonly Mock<ICityRepository> _cityRepoMock;
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IImageRepository> _imageRepoMock;
    private readonly Mock<ILogger<HotelQueryService>> _loggerMock;
    private readonly Mock<ISieveProcessor> _sieveMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly HotelQueryService _service;

    public HotelQueryServiceTests()
    {
        _hotelRepoMock = new Mock<IHotelRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _ownerRepoMock = new Mock<IOwnerRepository>();
        _cityRepoMock = new Mock<ICityRepository>();
        _reviewRepoMock = new Mock<IReviewRepository>();
        _imageRepoMock = new Mock<IImageRepository>();
        _loggerMock = new Mock<ILogger<HotelQueryService>>();
        _sieveMock = new Mock<ISieveProcessor>();
        _mapperMock = new Mock<IMapper>();

        _service = new HotelQueryService(
            _hotelRepoMock.Object,
            _userRepoMock.Object,
            _ownerRepoMock.Object,
            _cityRepoMock.Object,
            _reviewRepoMock.Object,
            _imageRepoMock.Object,
            _loggerMock.Object,
            _sieveMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldReturnHotels()
    {
        // Arrange
        var featuredHotels = new List<FeaturedHotelModel>
        {
            new FeaturedHotelModel { HotelId = 1, HotelName = "Hotel One", Location = "City A" },
            new FeaturedHotelModel { HotelId = 2, HotelName = "Hotel Two", Location = "City B" },
        };

        _hotelRepoMock
            .Setup(r => r.GetFeaturedDealsAsync(3))
            .ReturnsAsync(featuredHotels);

        // Act
        var result = await _service.GetFeaturedDealsAsync(3);

        // Assert
        result.Should().HaveCount(2);
        result.All(h => featuredHotels.Select(x => x.HotelId).Contains(h.HotelId)).Should().BeTrue();
    }

    [Fact]
    public async Task GetRecentVisitedHotelsAsync_ShouldReturnHotels_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = 5 };

        var visitedHotels = new List<VisitedHotelDto>
        {
            new VisitedHotelDto { HotelId = 1, HotelName = "Hotel 1", CityName = "City A", StarRating = 4, TotalPrice = 200 },
            new VisitedHotelDto { HotelId = 2, HotelName = "Hotel 2", CityName = "City B", StarRating = 5, TotalPrice = 300 }
        };

        var mapped = new List<RecentHotelDto>
        {
            new RecentHotelDto { HotelId = 1, HotelName = "Hotel 1" },
            new RecentHotelDto { HotelId = 2, HotelName = "Hotel 2" }
        };

        _userRepoMock.Setup(x => x.GetUserByIdAsync(user.UserId)).ReturnsAsync(user);
        _hotelRepoMock.Setup(x => x.GetRecentVisitedHotelsByUserIdAsync(user.UserId)).ReturnsAsync(visitedHotels);
        _mapperMock.Setup(x => x.Map<IEnumerable<RecentHotelDto>>(visitedHotels)).Returns(mapped);

        // Act
        var result = await _service.GetRecentVisitedHotelsAsync(user.UserId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentVisitedHotelsAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        // Act
        Func<Task> act = async () => await _service.GetRecentVisitedHotelsAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetHotelDetailsAsync_ShouldReturnHotelDetails()
    {
        // Arrange
        var hotel = new Hotel
        {
            HotelId = 1,
            Name = "Test Hotel",
            City = new City { Name = "City" },
            Owner = new Owner { FirstName = "Owner" },
            Reviews = new List<Review> { new Review(), new Review() }
        };

        _hotelRepoMock.Setup(x => x.GetHotelByIdAsync(hotel.HotelId)).ReturnsAsync(hotel);
        _reviewRepoMock.Setup(x => x.GetHotelRatingAsync(hotel.HotelId)).ReturnsAsync(4.5);
        _imageRepoMock.Setup(x => x.GetHotelImagesAsync(hotel.HotelId)).ReturnsAsync(new List<Image> { new Image(), new Image() });
        _mapperMock.Setup(x => x.Map<List<ImageResponseDto>>(It.IsAny<IEnumerable<Image>>()))
            .Returns(new List<ImageResponseDto> { new ImageResponseDto(), new ImageResponseDto() });
        _mapperMock.Setup(x => x.Map<List<ReviewResponseDto>>(hotel.Reviews))
            .Returns(new List<ReviewResponseDto> { new ReviewResponseDto(), new ReviewResponseDto() });

        // Act
        var result = await _service.GetHotelDetailsAsync(hotel.HotelId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(hotel.Name);
    }

    [Fact]
    public async Task GetHotelDetailsAsync_ShouldThrowNotFoundException_WhenHotelNotFound()
    {
        // Arrange
        _hotelRepoMock.Setup(x => x.GetHotelByIdAsync(It.IsAny<int>())).ReturnsAsync((Hotel)null);

        // Act
        Func<Task> act = async () => await _service.GetHotelDetailsAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetHotelsByOwnerIdAsync_ShouldReturnHotels()
    {
        // Arrange
        int ownerId = 1;
        var hotels = new List<Hotel> { new Hotel(), new Hotel(), new Hotel() };
        _ownerRepoMock.Setup(x => x.IsOwnerExistsAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(true);
        _hotelRepoMock.Setup(x => x.GetHotelsByOwnerIdAsync(ownerId)).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetHotelsByOwnerIdAsync(ownerId);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetHotelsByOwnerIdAsync_ShouldThrowNotFoundException_WhenOwnerNotFound()
    {
        // Arrange
        _ownerRepoMock.Setup(x => x.IsOwnerExistsAsync(It.IsAny<Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.GetHotelsByOwnerIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetHotelsByCityIdAsync_ShouldReturnPaginatedHotels()
    {
        // Arrange
        int cityId = 1;
        var pagination = new PaginationMetadata { PageNumber = 1, PageSize = 10 };
        var hotels = new List<Hotel> { new Hotel(), new Hotel() };
        var paginated = new PaginatedResult<Hotel>(hotels, pagination);

        _cityRepoMock.Setup(x => x.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(true);
        _hotelRepoMock.Setup(x => x.GetHotelsByCityIdAsync(cityId, pagination)).ReturnsAsync(paginated);
        _mapperMock.Setup(x => x.Map<ImageResponseDto>(It.IsAny<Image>())).Returns(new ImageResponseDto());

        // Act
        var result = await _service.GetHotelsByCityIdAsync(cityId, pagination);

        // Assert
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetHotelsByCityIdAsync_ShouldThrowNotFoundException_WhenCityNotFound()
    {
        // Arrange
        _cityRepoMock.Setup(x => x.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>()))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _service.GetHotelsByCityIdAsync(123, new PaginationMetadata());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

}

