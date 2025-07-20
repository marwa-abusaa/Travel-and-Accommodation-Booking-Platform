using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Core.Entities;
using TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly AppDbContext _context;

    public ImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Image> AddImageAsync(Image image)
    {
        var newImage = await _context.Images.AddAsync(image);
        return newImage.Entity;
    }

    public async Task DeleteImageByIdAsync(int imageId)
    {
        var image = await GetImageByIdAsync(imageId);
        if (image != null)
        {
            _context.Images.Remove(image);
        }
    }

    public async Task<IEnumerable<Image>> GetHotelImagesAsync(int hotelId)
    {
        return await _context.Images.Where(i => i.HotelId == hotelId).ToListAsync();
    }

    public async Task<Image?> GetImageByIdAsync(int imageId)
    {
        return await _context.Images.FindAsync(imageId);
    }

    public async Task<IEnumerable<Image>> GetRoomImagesAsync(int roomlId)
    {
        return await _context.Images.Where(i => i.RoomId == roomlId).ToListAsync();
    }

    public async Task UpdateImageAsync(Image image)
    {
        if (await GetImageByIdAsync(image.ImageId) != null)
        {
            _context.Images.Update(image);
        }
    }
}
