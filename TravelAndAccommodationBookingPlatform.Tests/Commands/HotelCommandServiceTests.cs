using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class HotelCommandServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<ICityRepository> _cityRepoMock = new();
    private readonly Mock<IOwnerRepository> _ownerRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<HotelCommandService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly HotelCommandService _service;

    public HotelCommandServiceTests()
    {
        _service = new HotelCommandService(
            _hotelRepoMock.Object,
            _cityRepoMock.Object,
            _ownerRepoMock.Object,
            _roomRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task AddHotelAsync_ShouldAddHotel_WhenValid()
    {
        // Arrange
        var dto = new CreateHotelDto
        {
            Name = "Test Hotel",
            CityId = 1,
            OwnerId = 2,
            StarRating = 4,
            Location = "Downtown"
        };

        var hotel = new Hotel { Name = dto.Name };
        var addedHotel = new Hotel { HotelId = 10, Name = dto.Name };
        var responseDto = new HotelResponseDto { HotelId = 10, Name = dto.Name };

        _cityRepoMock.Setup(c => c.IsCityExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<City, bool>>>())).ReturnsAsync(true);
        _ownerRepoMock.Setup(o => o.IsOwnerExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Owner, bool>>>())).ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<Hotel>(dto)).Returns(hotel);
        _hotelRepoMock.Setup(h => h.AddHotelAsync(hotel)).ReturnsAsync(addedHotel);
        _mapperMock.Setup(m => m.Map<HotelResponseDto>(addedHotel)).Returns(responseDto);

        // Act
        var result = await _service.AddHotelAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(10);
        result.Name.Should().Be("Test Hotel");
    }

    [Fact]
    public async Task AddHotelAsync_ShouldThrowNotFoundException_WhenCityNotFound()
    {
        // Arrange
        var createDto = new CreateHotelDto
        {
            Name = "Test Hotel",
            FullDescription = "Test Description",
            BriefDescription  = "Brief Test Description",
            Location = "Test Address",
            PhoneNumber = "123456789",
            StarRating = 4,
            CityId = 1,
            OwnerId = 1
        };

        _cityRepoMock.Setup(c => c.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>())).ReturnsAsync(false);
        _ownerRepoMock.Setup(o => o.IsOwnerExistsAsync(It.IsAny<Expression<Func<Owner, bool>>>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddHotelAsync(createDto));
    }

    [Fact]
    public async Task DeleteHotelByIdAsync_ShouldThrowNotFound_WhenHotelDoesNotExist()
    {
        // Arrange
        _hotelRepoMock.Setup(h => h.IsHotelExistsAsync(It.IsAny<Expression<Func<Hotel, bool>>>())).ReturnsAsync(false);

        // Act
        Func<Task> act = () => _service.DeleteHotelByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Hotel with ID '99' not found.");
    }

    [Fact]
    public async Task DeleteHotelByIdAsync_ShouldThrowBadRequest_WhenRoomsExist()
    {
        // Arrange
        _hotelRepoMock
            .Setup(h => h.IsHotelExistsAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
            .ReturnsAsync(true);

        _roomRepoMock
            .Setup(r => r.IsRoomExistsAsync(It.IsAny<Expression<Func<Room, bool>>>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = () => _service.DeleteHotelByIdAsync(1);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Hotel cannot be deleted because it has assigned rooms.");
    }

    [Fact]
    public async Task DeleteHotelByIdAsync_ShouldDeleteHotel_WhenHotelExistsAndNoRooms()
    {
        // Arrange
        _hotelRepoMock.Setup(h => h.IsHotelExistsAsync(It.IsAny<Expression<Func<Hotel, bool>>>())).ReturnsAsync(true);
        _roomRepoMock.Setup(r => r.IsRoomExistsAsync(It.IsAny<Expression<Func<Room, bool>>>())).ReturnsAsync(false);

        _hotelRepoMock.Setup(h => h.DeleteHotelByIdAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteHotelByIdAsync(1);

        // Assert
        _hotelRepoMock.Verify(h => h.DeleteHotelByIdAsync(1), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldThrowNotFound_WhenHotelNotFound()
    {
        // Arrange
        var dto = new UpdateHotelDto { HotelId = 100, CityId = 1, OwnerId = 1, StarRating = 3 };
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId)).ReturnsAsync((Hotel)null);

        // Act
        Func<Task> act = () => _service.UpdateHotelAsync(dto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Hotel with ID '100' not found.");
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldUpdateHotel_WhenValid()
    {
        // Arrange
        var dto = new UpdateHotelDto
        {
            HotelId = 1,
            CityId = 1,
            OwnerId = 1,
            Name = "Updated Hotel",
            StarRating = 4
        };

        var existingHotel = new Hotel { HotelId = dto.HotelId };

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId)).ReturnsAsync(existingHotel);
        _cityRepoMock.Setup(c => c.IsCityExistsAsync(It.IsAny<Expression<Func<City, bool>>>())).ReturnsAsync(true);
        _ownerRepoMock.Setup(o => o.IsOwnerExistsAsync(It.IsAny<Expression<Func<Owner, bool>>>())).ReturnsAsync(true);
        _hotelRepoMock.Setup(h => h.UpdateHotelAsync(existingHotel)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateHotelAsync(dto);

        // Assert
        _hotelRepoMock.Verify(h => h.UpdateHotelAsync(existingHotel), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

}
