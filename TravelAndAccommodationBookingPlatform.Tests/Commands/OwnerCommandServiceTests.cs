using Moq;
using AutoMapper;
using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class OwnerCommandServiceTests
{
    private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OwnerCommandService _service;

    public OwnerCommandServiceTests()
    {
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        var loggerMock = Mock.Of<Microsoft.Extensions.Logging.ILogger<OwnerCommandService>>();

        _service = new OwnerCommandService(
            _ownerRepositoryMock.Object,
            loggerMock,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task AddOwnerAsync_ShouldAddOwnerSuccessfully_WhenEmailDoesNotConflict()
    {
        // Arrange
        var createDto = new CreateOwnerDto
        {
            FirstName = "Marwa",
            LastName = "AbuSaa",
            Email = "marwa@example.com",
            PhoneNumber = "123456"
        };

        var ownerEntity = new Owner { OwnerId = 1, Email = createDto.Email };
        var responseDto = new OwnerResponseDto { OwnerId = 1, Email = createDto.Email };

        _ownerRepositoryMock
            .Setup(r => r.IsOwnerExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<Owner>(createDto)).Returns(ownerEntity);
        _ownerRepositoryMock.Setup(r => r.AddOwnerAsync(ownerEntity)).ReturnsAsync(ownerEntity);
        _mapperMock.Setup(m => m.Map<OwnerResponseDto>(ownerEntity)).Returns(responseDto);

        // Act
        var result = await _service.AddOwnerAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.OwnerId.Should().Be(1);
        result.Email.Should().Be(createDto.Email);
    }

    [Fact]
    public async Task AddOwnerAsync_ShouldThrowEmailAlreadyExistsException_WhenEmailExists()
    {
        // Arrange
        var createDto = new CreateOwnerDto { Email = "duplicate@example.com" };

        _ownerRepositoryMock
            .Setup(r => r.IsOwnerExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Owner, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _service.AddOwnerAsync(createDto));
    }

    [Fact]
    public async Task UpdateOwnerAsync_ShouldThrowNotFoundException_WhenOwnerDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateOwnerDto { OwnerId = 99 };

        _ownerRepositoryMock.Setup(r => r.GetOwnerByIdAsync(updateDto.OwnerId)).ReturnsAsync((Owner)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateOwnerAsync(updateDto));
    }

    [Fact]
    public async Task UpdateOwnerAsync_ShouldUpdateOwner_WhenOwnerExists()
    {
        // Arrange
        var updateDto = new UpdateOwnerDto { OwnerId = 1, FirstName = "Updated" };
        var ownerEntity = new Owner { OwnerId = 1, FirstName = "Old" };

        _ownerRepositoryMock.Setup(r => r.GetOwnerByIdAsync(updateDto.OwnerId)).ReturnsAsync(ownerEntity);
        _ownerRepositoryMock.Setup(r => r.UpdateOwnerAsync(ownerEntity)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateOwnerAsync(updateDto);

        // Assert 
        _mapperMock.Verify(m => m.Map(updateDto, ownerEntity), Times.Once);
        _ownerRepositoryMock.Verify(r => r.UpdateOwnerAsync(ownerEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
