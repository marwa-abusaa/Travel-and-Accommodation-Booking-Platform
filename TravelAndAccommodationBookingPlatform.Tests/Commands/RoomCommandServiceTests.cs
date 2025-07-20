using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class RoomCommandServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly RoomCommandService _service;

    public RoomCommandServiceTests()
    {
        _service = new RoomCommandService(
            _roomRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            Mock.Of<Microsoft.Extensions.Logging.ILogger<RoomCommandService>>() 
        );
    }

    [Fact]
    public async Task AddRoomAsync_ShouldReturnRoom_WhenHotelExists()
    {
        // Arrange
        var createDto = new CreateRoomDto { HotelId = 1 };
        var room = new Room { RoomId = 10 };
        var roomResponse = new RoomResponseDto { RoomId = 10 };

        _hotelRepositoryMock.Setup(r => r.IsHotelExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Hotel, bool>>>()))
                            .ReturnsAsync(true);
        _mapperMock.Setup(m => m.Map<Room>(createDto)).Returns(room);
        _roomRepositoryMock.Setup(r => r.AddRoomAsync(room)).ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map<RoomResponseDto>(room)).Returns(roomResponse);

        // Act
        var result = await _service.AddRoomAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.RoomId.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task AddRoomAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        var createDto = new CreateRoomDto { HotelId = 2 };

        _hotelRepositoryMock.Setup(r => r.IsHotelExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Hotel, bool>>>()))
                            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddRoomAsync(createDto));
    }


    [Fact]
    public async Task DeleteRoomByIdAsync_ShouldDeleteRoom_WhenRoomExists()
    {
        // Arrange
        int roomId = 5;

        _roomRepositoryMock.Setup(r => r.IsRoomExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Room, bool>>>()))
                           .ReturnsAsync(true);

        // Act
        await _service.DeleteRoomByIdAsync(roomId);

        // Assert
        _roomRepositoryMock.Verify(r => r.DeleteRoomByIdAsync(roomId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomByIdAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        int roomId = 100;

        _roomRepositoryMock.Setup(r => r.IsRoomExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Room, bool>>>()))
                           .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteRoomByIdAsync(roomId));
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldUpdateRoom_WhenRoomExists()
    {
        // Arrange
        var updateDto = new UpdateRoomDto { RoomId = 20 };
        var existingRoom = new Room { RoomId = 20 };

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(updateDto.RoomId)).ReturnsAsync(existingRoom);

        // Act
        await _service.UpdateRoomAsync(updateDto);

        // Assert
        _mapperMock.Verify(m => m.Map(updateDto, existingRoom), Times.Once);
        _roomRepositoryMock.Verify(r => r.UpdateRoomAsync(existingRoom), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
   

    [Fact]
    public async Task UpdateRoomAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateRoomDto { RoomId = 30 };

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(updateDto.RoomId)).ReturnsAsync((Room?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateRoomAsync(updateDto));
    }
}
