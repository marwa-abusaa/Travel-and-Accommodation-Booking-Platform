using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Core.Models;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Queries;

public interface IDiscountQueryService
{
    Task<DiscountResponseDto?> GetDiscountByIdAsync(int discountId);
    Task<IEnumerable<DiscountResponseDto>> GetDiscountByRoomIdAsync(int roomId); 
    Task<PaginatedResult<DiscountResponseDto>> GetDiscountsAsync(PaginationMetadata pagination);
}