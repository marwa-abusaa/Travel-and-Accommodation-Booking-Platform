using AutoMapper;
using Microsoft.Extensions.Logging;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;
using TravelAndAccommodationBookingPlatform.Core.Exceptions;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

namespace TravelAndAccommodationBookingPlatform.Application.Services.Queries;

public class ImageQueryService : IImageQueryService
{
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<ImageQueryService> _logger;
    private readonly IMapper _mapper;

    public ImageQueryService(
        IImageRepository imageRepository,
        ILogger<ImageQueryService> logger,
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ImageResponseDto?> GetImageByIdAsync(int imageId)
    {
        var image = await _imageRepository.GetImageByIdAsync(imageId);
        if (image is null)
        {
            _logger.LogWarning("Image with ID {ImageId} was not found.", imageId);
            throw new NotFoundException($"Image with ID '{imageId}' not found.");
        }

        _logger.LogInformation("Successfully fetched image with ID: {ImageId}", imageId);

        return _mapper.Map<ImageResponseDto>(image);
    }
}
