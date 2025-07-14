using AutoMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Enums;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.UnitOfWork;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Commands;

public class ImageCommandService : IImageCommandService
{
    private readonly IImageRepository _imageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ImageCommandService> _logger;
    private readonly IMapper _mapper;

    public ImageCommandService(
        IImageRepository imageRepository,
        IHotelRepository hotelRepository,
        IRoomRepository roomRepository, 
        IUnitOfWork unitOfWork,
        ILogger<ImageCommandService> logger, 
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ImageResponseDto> AddImageAsync(CreateImageDto createImageDto)
    {
        _logger.LogInformation("Starting image creation process.");

        var hotel = await _hotelRepository.GetHotelByIdAsync(createImageDto.HotelId);
        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", createImageDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{createImageDto.HotelId}' not found.");
        } 

        if (createImageDto.Type == ImageType.Room)
        {
            if (!createImageDto.RoomId.HasValue)
            {
                _logger.LogWarning("Room ID is required for image type Room.");
                throw new ValidationException("Room ID is required for Room images.");
            }

            var room = await _roomRepository.GetRoomByIdAsync(createImageDto.RoomId.Value);
            if (room is null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found.", createImageDto.RoomId.Value);
                throw new NotFoundException($"Room with ID '{createImageDto.RoomId}' not found.");
            }

            if (room.HotelId != createImageDto.HotelId)
            {
                _logger.LogWarning("Room {RoomId} does not belong to hotel {HotelId}.", room.RoomId, createImageDto.HotelId);
                throw new ValidationException("The room does not belong to the specified hotel.");
            }
        }

        var image = _mapper.Map<Image>(createImageDto);
        var addedImage = await _imageRepository.AddImageAsync(image);
        await _unitOfWork.SaveChangesAsync(); 

        if (createImageDto.Type == ImageType.HotelThumbnail)
        {
            hotel.ThumbnailId = image.ImageId;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Image {ImageId} set as hotel {HotelId} thumbnail.", image.ImageId, hotel.HotelId);
        }

        _logger.LogInformation("Image with ID {ImageId} successfully created.", addedImage.ImageId);

        return _mapper.Map<ImageResponseDto>(addedImage);
    }

    public async Task DeleteImageByIdAsync(int imageId)
    {
        var image = await _imageRepository.GetImageByIdAsync(imageId);
        if (image is null)
        {
            _logger.LogWarning("Image with ID {ImageId} not found.", imageId);
            throw new NotFoundException($"Image with ID '{imageId}' not found.");
        }

        var hotel = await _hotelRepository.GetHotelByIdAsync(image.HotelId);
        if (hotel != null && hotel.ThumbnailId == image.ImageId)
        {
            hotel.ThumbnailId = null;
            _logger.LogInformation("Removed thumbnail reference from hotel {HotelId}", hotel.HotelId);
        }

        await _imageRepository.DeleteImageByIdAsync(imageId);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Image with ID {ImageId} successfully deleted.", imageId);
    }

    public async Task UpdateImageAsync(UpdateImageDto updateImageDto)
    {
        var existingImage = await _imageRepository.GetImageByIdAsync(updateImageDto.ImageId);
        if (existingImage is null)
        {
            _logger.LogWarning("Image with ID {ImageId} not found.", updateImageDto.ImageId);
            throw new NotFoundException($"Image with ID '{updateImageDto.ImageId}' not found.");
        }

        var hotel = await _hotelRepository.GetHotelByIdAsync(updateImageDto.HotelId);
        if (hotel is null)
        {
            _logger.LogWarning("Hotel with ID {HotelId} not found.", updateImageDto.HotelId);
            throw new NotFoundException($"Hotel with ID '{updateImageDto.HotelId}' not found.");
        }

        if (updateImageDto.Type == ImageType.Room)
        {
            var room = await _roomRepository.GetRoomByIdAsync(updateImageDto.RoomId);
            if (room is null)
            {
                _logger.LogWarning("Room with ID {RoomId} not found.", updateImageDto.RoomId);
                throw new NotFoundException($"Room with ID '{updateImageDto.RoomId}' not found.");
            }

            if (room.HotelId != updateImageDto.HotelId)
            {
                _logger.LogWarning("Room {RoomId} does not belong to hotel {HotelId}.", room.RoomId, hotel.HotelId);
                throw new ValidationException("The room does not belong to the specified hotel.");
            }
        }

        _mapper.Map(updateImageDto, existingImage);

        await _imageRepository.UpdateImageAsync(existingImage);
        await _unitOfWork.SaveChangesAsync();

        if (updateImageDto.Type == ImageType.HotelThumbnail)
        {
            hotel.ThumbnailId = existingImage.ImageId;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Updated image {ImageId} set as thumbnail for hotel {HotelId}.", existingImage.ImageId, hotel.HotelId);
        }

        _logger.LogInformation("Image with ID {ImageId} successfully updated.", existingImage.ImageId);
    }
}
