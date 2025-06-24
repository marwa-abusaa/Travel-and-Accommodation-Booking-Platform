using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IImageRepository
{
    public Task<IEnumerable<Image>> GetHotelImagesAsync(int hotelId);
    public Task<IEnumerable<Image>> GetRoomImagesAsync(int roomlId);
    public Task<Image?> GetImageByIdAsync(int imageId);
    public Task<Image> AddImageAsync(Image image);
    public Task DeleteImageAsync(int imageId);
    public Task UpdateImageAsync(Image image);
}
