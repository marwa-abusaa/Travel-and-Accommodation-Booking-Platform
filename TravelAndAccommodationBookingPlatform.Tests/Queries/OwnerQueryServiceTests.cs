using Moq;
using FluentAssertions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class OwnerQueryServiceTests
{
    private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
    private readonly Mock<ILogger<OwnerQueryService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OwnerQueryService _service;

    public OwnerQueryServiceTests()
    {
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _loggerMock = new Mock<ILogger<OwnerQueryService>>();
        _mapperMock = new Mock<IMapper>();
        _service = new OwnerQueryService(
            _ownerRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetOwnerByHotelIdAsync_ShouldReturnOwner_WhenHotelExists()
    {
        // Arrange
        var hotelId = 1;
        var owner = new Owner { OwnerId = 1, FirstName = "John" };
        var ownerDto = new OwnerResponseDto { OwnerId = 1, FirstName = "John" };

        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByHotelIdAsync(hotelId))
            .ReturnsAsync(owner);

        _mapperMock
            .Setup(mapper => mapper.Map<OwnerResponseDto>(owner))
            .Returns(ownerDto);

        // Act
        var result = await _service.GetOwnerByHotelIdAsync(hotelId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(ownerDto);
    }

    [Fact]
    public async Task GetOwnerByHotelIdAsync_ShouldThrowNotFoundException_WhenHotelNotFound()
    {
        // Arrange
        var hotelId = 1;
        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByHotelIdAsync(hotelId))
            .ReturnsAsync((Owner?)null);

        // Act
        var act = async () => await _service.GetOwnerByHotelIdAsync(hotelId);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"No owner found for hotel with ID '{hotelId}'.");
    }

    [Fact]
    public async Task GetOwnerByIdAsync_ShouldReturnOwner_WhenOwnerExists()
    {
        // Arrange
        var ownerId = 1;
        var owner = new Owner { OwnerId = 1, FirstName = "Alice" };
        var ownerDto = new OwnerResponseDto { OwnerId = 1, FirstName = "Alice" };

        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByIdAsync(ownerId))
            .ReturnsAsync(owner);

        _mapperMock
            .Setup(mapper => mapper.Map<OwnerResponseDto>(owner))
            .Returns(ownerDto);

        // Act
        var result = await _service.GetOwnerByIdAsync(ownerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(ownerDto);
    }

    [Fact]
    public async Task GetOwnerByIdAsync_ShouldThrowNotFoundException_WhenOwnerNotFound()
    {
        // Arrange
        var ownerId = 1;
        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByIdAsync(ownerId))
            .ReturnsAsync((Owner?)null);

        // Act
        var act = async () => await _service.GetOwnerByIdAsync(ownerId);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Owner with ID '{ownerId}' not found.");
    }
}
