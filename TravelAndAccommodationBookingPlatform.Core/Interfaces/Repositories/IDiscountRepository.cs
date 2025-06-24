using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Core.Interfaces.Repositories;

public interface IDiscountRepository
{
    public Task<Discount> AddDiscountAsync(Discount discount);
    public Task<Discount?> GetDiscountByIdAsync(int discountId, int roomId);
    public Task DeleteDiscountAsync(int discountId);
    public IQueryable GetAllDiscounts();
}