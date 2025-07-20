using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;


namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class DiscountQueryServiceTests
{
    private readonly Mock<IDiscountRepository> _discountRepoMock;
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<DiscountQueryService>> _loggerMock;
    private readonly DiscountQueryService _service;

    public DiscountQueryServiceTests()
    {
        _discountRepoMock = new Mock<IDiscountRepository>();
        _roomRepoMock = new Mock<IRoomRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<DiscountQueryService>>();

        _service = new DiscountQueryService(
            _discountRepoMock.Object,
            _roomRepoMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetDiscountByIdAsync_ShouldReturnDiscount_WhenFound()
    {
        // Arrange
        var discountId = 1;
        var discount = new Discount { DiscountId = discountId };
        var dto = new DiscountResponseDto { DiscountId = discountId };

        _discountRepoMock.Setup(r => r.GetDiscountByIdAsync(discountId)).ReturnsAsync(discount);
        _mapperMock.Setup(m => m.Map<DiscountResponseDto>(discount)).Returns(dto);

        // Act
        var result = await _service.GetDiscountByIdAsync(discountId);

        // Assert
        result.Should().NotBeNull();
        result!.DiscountId.Should().Be(discountId);
    }

    [Fact]
    public async Task GetDiscountByIdAsync_ShouldThrowNotFound_WhenDiscountNotFound()
    {
        // Arrange
        var discountId = 999;
        _discountRepoMock.Setup(r => r.GetDiscountByIdAsync(discountId)).ReturnsAsync((Discount?)null);

        // Act
        Func<Task> act = async () => await _service.GetDiscountByIdAsync(discountId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Discount with ID '{discountId}' not found.");
    }

    [Fact]
    public async Task GetDiscountsByRoomIdAsync_ShouldReturnDiscounts_WhenRoomExists()
    {
        // Arrange
        var roomId = 10;
        var discounts = new List<Discount> { new Discount { DiscountId = 1, RoomId = roomId } };
        var dtos = new List<DiscountResponseDto> { new DiscountResponseDto { DiscountId = 1, RoomId = roomId } };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(roomId)).ReturnsAsync(new Room { RoomId = roomId });
        _discountRepoMock.Setup(r => r.GetDiscountByRoomIdAsync(roomId)).ReturnsAsync(discounts);
        _mapperMock.Setup(m => m.Map<List<DiscountResponseDto>>(discounts)).Returns(dtos);

        // Act
        var result = await _service.GetDiscountsByRoomIdAsync(roomId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RoomId.Should().Be(roomId);
    }

    [Fact]
    public async Task GetDiscountsByRoomIdAsync_ShouldThrowNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = 999;
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(roomId)).ReturnsAsync((Room?)null);

        // Act
        Func<Task> act = async () => await _service.GetDiscountsByRoomIdAsync(roomId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Room with ID '{roomId}' not found.");
    }
}
