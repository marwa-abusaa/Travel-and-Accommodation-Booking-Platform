using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Bookings;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Services;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Application.Mapping;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class BookingCommandServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock = new();
    private readonly Mock<IBookingCreationService> _creationServiceMock = new();
    private readonly Mock<IBookingConfirmationService> _confirmationServiceMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<BookingCommandService>> _loggerMock = new();
    private readonly IMapper _mapper;

    private readonly BookingCommandService _service;

    public BookingCommandServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Booking, BookingResponseDto>();
            cfg.AddProfile<RoomProfile>();
        });

        _mapper = config.CreateMapper();

        _service = new BookingCommandService(
            _bookingRepoMock.Object,
            _creationServiceMock.Object,
            _confirmationServiceMock.Object,
            _roomRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapper
        );
    }

    [Fact]
    public async Task AddBookingAsync_ShouldCreateAndReturnBooking_WhenValid()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            UserId = 1,
            RoomIds = new List<int> { 101 },
            Remarks = "Sea view",
            CheckInDate = new DateOnly(2025, 08, 01),
            CheckOutDate = new DateOnly(2025, 08, 05),
            PaymentType = PaymentType.Visa
        };

        var room = new Room { RoomId = 101, PricePerNight = 100 };
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(101)).ReturnsAsync(room);

        var createdBooking = new Booking
        {
            BookingId = 55,
            UserId = dto.UserId,
            Rooms = new List<Room> { room },
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPriceAfterDiscount = 400
        };

        _creationServiceMock.Setup(s => s.CreateBookingAsync(It.IsAny<Booking>())).ReturnsAsync(createdBooking);
        _bookingRepoMock
                .Setup(r => r.AddBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync((Booking booking) => booking);
        _confirmationServiceMock.Setup(s => s.SendBookingConfirmationAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddBookingAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.BookingId.Should().Be(55);
        result.TotalPriceAfterDiscount.Should().Be(400);
    }

    [Fact]
    public async Task AddBookingAsync_ShouldThrowNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var dto = new CreateBookingDto
        {
            UserId = 1,
            RoomIds = new List<int> { 999 },
            CheckInDate = new DateOnly(2025, 08, 01),
            CheckOutDate = new DateOnly(2025, 08, 05),
            PaymentType = PaymentType.PayPal
        };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(999)).ReturnsAsync((Room?)null);

        // Act
        var act = async () => await _service.AddBookingAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Room with ID 999 not found");
    }

    [Fact]
    public async Task DeleteBookingByIdAsync_ShouldDelete_WhenBookingExists()
    {
        // Arrange
        var booking = new Booking { BookingId = 77 };
        _bookingRepoMock.Setup(r => r.GetBookingByIdAsync(77)).ReturnsAsync(booking);
        _bookingRepoMock.Setup(r => r.DeleteBookingByIdAsync(77)).Returns(Task.CompletedTask);

        // Act
        var act = async () => await _service.DeleteBookingByIdAsync(77);

        // Assert
        await act.Should().NotThrowAsync();
        _bookingRepoMock.Verify(r => r.DeleteBookingByIdAsync(77), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingByIdAsync_ShouldThrowNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        _bookingRepoMock.Setup(r => r.GetBookingByIdAsync(123)).ReturnsAsync((Booking?)null);

        // Act
        var act = async () => await _service.DeleteBookingByIdAsync(123);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Booking with ID 123 not found.");
    }
}
