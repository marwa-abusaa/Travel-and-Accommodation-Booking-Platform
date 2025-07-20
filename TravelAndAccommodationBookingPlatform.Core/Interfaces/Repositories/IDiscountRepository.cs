using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IDiscountRepository
{
    Task<Discount> AddDiscountAsync(Discount discount);
    Task<Discount?> GetDiscountByIdAsync(int discountId);
    Task<IEnumerable<Discount>> GetDiscountByRoomIdAsync(int roomId);
    Task DeleteDiscountByIdAsync(int discountId);
    Task<Discount?> GetBestValidDiscountForRoomAsync(int roomId, DateTime checkIn, DateTime checkOut);
}