using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IDiscountQueryService
{
    Task<DiscountResponseDto?> GetDiscountByIdAsync(int discountId);
    Task<IEnumerable<DiscountResponseDto>> GetDiscountsByRoomIdAsync(int roomId); 
}