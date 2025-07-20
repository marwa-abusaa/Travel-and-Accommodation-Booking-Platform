using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IImageCommandService
{
    Task<ImageResponseDto> AddImageAsync(CreateImageDto createImageDto);
    Task DeleteImageByIdAsync(int imageId);
    Task UpdateImageAsync(UpdateImageDto updateImageDto);
}