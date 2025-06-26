using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IImageRepository
{
    Task<IEnumerable<Image>> GetHotelImagesAsync(int hotelId);
    Task<IEnumerable<Image>> GetRoomImagesAsync(int roomlId);
    Task<Image?> GetImageByIdAsync(int imageId);
    Task<Image> AddImageAsync(Image image);
    Task DeleteImageByIdAsync(int imageId);
    Task UpdateImageAsync(Image image);
}
