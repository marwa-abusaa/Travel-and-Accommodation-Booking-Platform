using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;
using TravelAndAccommodationBookingPlatform.Application.Services.Commands;
using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Commands;

public class ImageCommandServiceTests
{
    private readonly Mock<IImageRepository> _imageRepoMock = new();
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<ImageCommandService>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly ImageCommandService _service;

    public ImageCommandServiceTests()
    {
        _service = new ImageCommandService(
            _imageRepoMock.Object,
            _hotelRepoMock.Object,
            _roomRepoMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _mapperMock.Object);
    }


    [Fact]
    public async Task AddImageAsync_ShouldAddHotelThumbnail_WhenHotelExists()
    {
        // Arrange
        var createDto = new CreateImageDto
        {
            HotelId = 1,
            Type = ImageType.HotelThumbnail
        };
        var hotel = new Hotel { HotelId = 1 };
        var image = new Image { ImageId = 10, HotelId = 1 };
        var responseDto = new ImageResponseDto { ImageId = 10 };

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(createDto.HotelId))
            .ReturnsAsync(hotel);

        _mapperMock.Setup(m => m.Map<Image>(createDto)).Returns(image);

        _imageRepoMock.Setup(i => i.AddImageAsync(image)).ReturnsAsync(image);


        _mapperMock.Setup(m => m.Map<ImageResponseDto>(image)).Returns(responseDto);

        // Act
        var result = await _service.AddImageAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(responseDto);
        _hotelRepoMock.Verify(h => h.GetHotelByIdAsync(createDto.HotelId), Times.Once);
        _imageRepoMock.Verify(i => i.AddImageAsync(image), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task AddImageAsync_ShouldThrowNotFoundException_WhenHotelNotFound()
    {
        // Arrange
        var createDto = new CreateImageDto { HotelId = 1 };

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(createDto.HotelId))
            .ReturnsAsync((Hotel)null);

        // Act
        Func<Task> act = async () => await _service.AddImageAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Hotel with ID '{createDto.HotelId}' not found.");
    }

 
    [Fact]
    public async Task AddImageAsync_ShouldThrowNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var createDto = new CreateImageDto
        {
            HotelId = 1,
            Type = ImageType.Room,
            RoomId = 5
        };

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(createDto.HotelId))
            .ReturnsAsync(new Hotel { HotelId = 1 });

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(createDto.RoomId.Value))
            .ReturnsAsync((Room)null);

        // Act
        Func<Task> act = async () => await _service.AddImageAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Room with ID '{createDto.RoomId}' not found.");
    }



    [Fact]
    public async Task DeleteImageByIdAsync_ShouldDeleteImage_WhenImageExists()
    {
        // Arrange
        var imageId = 10;
        var image = new Image { ImageId = imageId, HotelId = 1 };
        var hotel = new Hotel { HotelId = 1, ThumbnailId = imageId };

        _imageRepoMock.Setup(i => i.GetImageByIdAsync(imageId)).ReturnsAsync(image);
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(image.HotelId)).ReturnsAsync(hotel);
        _imageRepoMock.Setup(i => i.DeleteImageByIdAsync(imageId)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteImageByIdAsync(imageId);

        // Assert
        _hotelRepoMock.Verify(h => h.GetHotelByIdAsync(image.HotelId), Times.Once);
        _imageRepoMock.Verify(i => i.DeleteImageByIdAsync(imageId), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        hotel.ThumbnailId.Should().BeNull();
    }

    [Fact]
    public async Task DeleteImageByIdAsync_ShouldThrowNotFoundException_WhenImageNotFound()
    {
        // Arrange
        _imageRepoMock.Setup(i => i.GetImageByIdAsync(It.IsAny<int>())).ReturnsAsync((Image)null);

        // Act
        Func<Task> act = async () => await _service.DeleteImageByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Image with ID '99' not found.");
    }


    [Fact]
    public async Task UpdateImageAsync_ShouldUpdateImage_WhenDataIsValid()
    {
        // Arrange
        var updateDto = new UpdateImageDto
        {
            ImageId = 10,
            HotelId = 1,
            Type = ImageType.HotelThumbnail
        };
        var existingImage = new Image { ImageId = 10, HotelId = 1 };
        var hotel = new Hotel { HotelId = 1 };

        _imageRepoMock.Setup(i => i.GetImageByIdAsync(updateDto.ImageId)).ReturnsAsync(existingImage);
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(updateDto.HotelId)).ReturnsAsync(hotel);
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(It.IsAny<int>())).ReturnsAsync(new Room { RoomId = 1, HotelId = 1 });

        _mapperMock.Setup(m => m.Map(updateDto, existingImage));

        _imageRepoMock.Setup(i => i.UpdateImageAsync(existingImage)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateImageAsync(updateDto);

        // Assert
        _imageRepoMock.Verify(i => i.UpdateImageAsync(existingImage), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        hotel.ThumbnailId.Should().Be(existingImage.ImageId);
    }

    [Fact]
    public async Task UpdateImageAsync_ShouldThrowNotFoundException_WhenImageNotFound()
    {
        // Arrange
        var updateDto = new UpdateImageDto { ImageId = 10, HotelId = 1 };

        _imageRepoMock.Setup(i => i.GetImageByIdAsync(updateDto.ImageId)).ReturnsAsync((Image)null);

        // Act
        Func<Task> act = async () => await _service.UpdateImageAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Image with ID '{updateDto.ImageId}' not found.");
    }
    
}
