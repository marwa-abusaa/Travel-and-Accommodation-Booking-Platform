using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class BookingQueryServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<BookingQueryService>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly BookingQueryService _service;

    public BookingQueryServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<BookingQueryService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Booking, BookingResponseDto>();
        });
        _mapper = config.CreateMapper();

        _service = new BookingQueryService(
            _bookingRepositoryMock.Object,
            _userRepositoryMock.Object,
            _loggerMock.Object,
            _mapper
        );
    }

    [Fact]
    public async Task GetBookingByIdAsync_ShouldReturnBooking_WhenFound()
    {
        // Arrange
        var booking = new Booking { BookingId = 1 };
        _bookingRepositoryMock.Setup(r => r.GetBookingByIdAsync(1)).ReturnsAsync(booking);

        // Act
        var result = await _service.GetBookingByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.BookingId.Should().Be(1);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ShouldThrowNotFound_WhenBookingMissing()
    {
        // Arrange
        _bookingRepositoryMock.Setup(r => r.GetBookingByIdAsync(1)).ReturnsAsync((Booking?)null);

        // Act
        var act = async () => await _service.GetBookingByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Booking with ID '1' was not found.");
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnBookings_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = 10 };
        var bookings = new List<Booking> { new Booking { BookingId = 1 }, new Booking { BookingId = 2 } };

        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(10)).ReturnsAsync(user);
        _bookingRepositoryMock.Setup(r => r.GetBookingsByUserIdAsync(10)).ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsByUserIdAsync(10);

        // Assert
        result.Should().HaveCount(2);
        result.Select(b => b.BookingId).Should().Contain(new[] { 1, 2 });
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldThrowNotFound_WhenUserMissing()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(10)).ReturnsAsync((User?)null);

        // Act
        var act = async () => await _service.GetBookingsByUserIdAsync(10);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User with ID '10' was not found.");
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ShouldReturnEmpty_WhenNoBookingsFound()
    {
        // Arrange
        var user = new User { UserId = 15 };
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(15)).ReturnsAsync(user);
        _bookingRepositoryMock.Setup(r => r.GetBookingsByUserIdAsync(15)).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetBookingsByUserIdAsync(15);

        // Assert
        result.Should().BeEmpty();
    }
}
