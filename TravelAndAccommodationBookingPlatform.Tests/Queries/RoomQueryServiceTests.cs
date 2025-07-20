using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class RoomQueryServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IHotelRepository> _hotelRepositoryMock = new();
    private readonly Mock<IImageRepository> _imageRepositoryMock = new();
    private readonly Mock<IDiscountRepository> _discountRepositoryMock = new();
    private readonly Mock<ILogger<RoomQueryService>> _loggerMock = new();
    private readonly Mock<ISieveProcessor> _sieveProcessorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly RoomQueryService _service;

    public RoomQueryServiceTests()
    {
        _service = new RoomQueryService(
            _roomRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _discountRepositoryMock.Object,
            _loggerMock.Object,
            _sieveProcessorMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetAvailableRoomsByHotelIdAsync_ShouldReturnPaginatedResult_WhenHotelExists()
    {
        // Arrange
        int hotelId = 1;
        var pagination = new PaginationMetadata { PageNumber = 1, PageSize = 10 };

        _hotelRepositoryMock
            .Setup(h => h.IsHotelExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Hotel, bool>>>()))
            .ReturnsAsync(true);

        var roomsPaginated = new PaginatedResult<Room>
        (
            new List<Room> { new Room { RoomId = 1 }, new Room { RoomId = 2 } },
            pagination
        );

        _roomRepositoryMock.Setup(r => r.GetAvailableRoomsByHotelIdAsync(hotelId, pagination))
                           .ReturnsAsync(roomsPaginated);

        var roomDtos = new List<RoomResponseDto>
        {
            new() { RoomId = 1 },
            new() { RoomId = 2 }
        };

        _mapperMock.Setup(m => m.Map<List<RoomResponseDto>>(It.IsAny<IEnumerable<Room>>()))
                   .Returns(roomDtos);

        // Act
        var result = await _service.GetAvailableRoomsByHotelIdAsync(hotelId, pagination);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.PaginationMetadata.PageNumber.Should().Be(1);
        result.PaginationMetadata.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetAvailableRoomsByHotelIdAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        int hotelId = 99;
        _hotelRepositoryMock
            .Setup(h => h.IsHotelExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Hotel, bool>>>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetAvailableRoomsByHotelIdAsync(hotelId, new PaginationMetadata()));
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnRoomDetails_WhenRoomExists()
    {
        // Arrange
        int roomId = 5;
        var room = new Room
        {
            RoomId = roomId,
            Description = "Nice room",
            PricePerNight = 100,
            HotelId = 1,
            AdultCapacity = 2,
            ChildrenCapacity = 1,
            RoomClass = RoomClass.Deluxe
        };
        var discounts = new List<Discount> { new Discount() };
        var images = new List<Image> { new Image() };

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId)).ReturnsAsync(room);

        _discountRepositoryMock.Setup(d => d.GetDiscountByRoomIdAsync(roomId)).ReturnsAsync(discounts);

        _imageRepositoryMock.Setup(i => i.GetRoomImagesAsync(roomId)).ReturnsAsync(images);

        var roomDetailsDto = new RoomDetailsDto { RoomId = roomId };
        var discountDtos = new List<DiscountResponseDto> { new DiscountResponseDto() };
        var imageDtos = new List<ImageResponseDto> { new ImageResponseDto() };

        _mapperMock.Setup(m => m.Map<RoomDetailsDto>(room)).Returns(roomDetailsDto);
        _mapperMock.Setup(m => m.Map<List<DiscountResponseDto>>(discounts)).Returns(discountDtos);
        _mapperMock.Setup(m => m.Map<List<ImageResponseDto>>(images)).Returns(imageDtos);

        // Act
        var result = await _service.GetRoomByIdAsync(roomId);

        // Assert
        result.Should().NotBeNull();
        result.RoomId.Should().Be(roomId);
        result.Discounts.Should().BeEquivalentTo(discountDtos);
        result.Images.Should().BeEquivalentTo(imageDtos);
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        int roomId = 10;
        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId)).ReturnsAsync((Room?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetRoomByIdAsync(roomId));
    }

}
