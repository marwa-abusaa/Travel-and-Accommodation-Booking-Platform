using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Services.Queries;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Queries;

public class ImageQueryServiceTests
{
    private readonly Mock<IImageRepository> _imageRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<ImageQueryService>> _loggerMock = new();

    private ImageQueryService CreateService()
    {
        return new ImageQueryService(_imageRepoMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetImageByIdAsync_ShouldReturnImageDto_WhenImageExists()
    {
        // Arrange
        var service = CreateService();

        var image = new Image
        {
            ImageId = 1,
            Name = "Sample Image",
            Path = "/images/sample.jpg",
            Type = ImageType.HotelThumbnail
        };

        var dto = new ImageResponseDto
        {
            ImageId = 1,
            Name = "Sample Image",
            Path = "/images/sample.jpg",
            Type = ImageType.HotelThumbnail
        };

        _imageRepoMock.Setup(x => x.GetImageByIdAsync(1)).ReturnsAsync(image);
        _mapperMock.Setup(m => m.Map<ImageResponseDto>(image)).Returns(dto);

        // Act
        var result = await service.GetImageByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task GetImageByIdAsync_ShouldThrowNotFoundException_WhenImageDoesNotExist()
    {
        // Arrange
        var service = CreateService();

        _imageRepoMock.Setup(x => x.GetImageByIdAsync(It.IsAny<int>())).ReturnsAsync((Image?)null);

        // Act
        var act = async () => await service.GetImageByIdAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}
