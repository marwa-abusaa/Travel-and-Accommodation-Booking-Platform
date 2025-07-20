using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;


namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class DiscountCommandServiceTests
{
    private readonly Mock<IDiscountRepository> _discountRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<DiscountCommandService>> _loggerMock = new();
    private readonly IMapper _mapper;
    private readonly DiscountCommandService _sut;

    public DiscountCommandServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateDiscountDto, Discount>();
            cfg.CreateMap<Discount, DiscountResponseDto>();
        });
        _mapper = config.CreateMapper();

        _sut = new DiscountCommandService(
            _discountRepoMock.Object,
            _roomRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapper);
    }

    [Fact]
    public async Task AddDiscountAsync_ShouldAddDiscount_WhenRoomExists()
    {
        // Arrange
        var createDto = new CreateDiscountDto
        {
            RoomId = 1,
            Percentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };

        var room = new Room { RoomId = createDto.RoomId };

        var discount = new Discount
        {
            DiscountId = 100,
            RoomId = createDto.RoomId,
            Percentage = createDto.Percentage,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
        };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(createDto.RoomId))
            .ReturnsAsync(room);

        _discountRepoMock.Setup(d => d.AddDiscountAsync(It.IsAny<Discount>()))
            .ReturnsAsync(discount);

        // Act
        var result = await _sut.AddDiscountAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.DiscountId.Should().Be(discount.DiscountId);
        result.RoomId.Should().Be(createDto.RoomId);
        result.Percentage.Should().Be(createDto.Percentage);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _roomRepoMock.Verify(r => r.GetRoomByIdAsync(createDto.RoomId), Times.Once);
        _discountRepoMock.Verify(d => d.AddDiscountAsync(It.IsAny<Discount>()), Times.Once);
    }

    [Fact]
    public async Task AddDiscountAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var createDto = new CreateDiscountDto { RoomId = 99 };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(createDto.RoomId))
            .ReturnsAsync((Room?)null);

        // Act
        Func<Task> act = () => _sut.AddDiscountAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Room with ID '{createDto.RoomId}' not found.");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        _discountRepoMock.Verify(d => d.AddDiscountAsync(It.IsAny<Discount>()), Times.Never);
    }

    [Fact]
    public async Task DeleteDiscountByIdAsync_ShouldDeleteDiscount_WhenDiscountExists()
    {
        // Arrange
        int discountId = 5;
        var discount = new Discount { DiscountId = discountId, RoomId = 1 };

        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(discountId))
            .ReturnsAsync(discount);

        // Act
        await _sut.DeleteDiscountByIdAsync(discountId);

        // Assert
        _discountRepoMock.Verify(d => d.DeleteDiscountByIdAsync(discountId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteDiscountByIdAsync_ShouldThrowNotFoundException_WhenDiscountDoesNotExist()
    {
        // Arrange
        int discountId = 99;

        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(discountId))
            .ReturnsAsync((Discount?)null);

        // Act
        Func<Task> act = () => _sut.DeleteDiscountByIdAsync(discountId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Discount with ID '{discountId}' not found.");
        _discountRepoMock.Verify(d => d.DeleteDiscountByIdAsync(It.IsAny<int>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
