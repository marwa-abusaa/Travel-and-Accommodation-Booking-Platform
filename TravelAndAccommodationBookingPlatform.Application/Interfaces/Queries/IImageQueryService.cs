using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IImageQueryService
{
    Task<ImageResponseDto?> GetImageByIdAsync(int imageId);
}
