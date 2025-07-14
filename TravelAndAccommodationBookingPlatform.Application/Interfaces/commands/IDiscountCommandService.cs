using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;


namespace TravelAndAccommodationBookingPlatform.Application.Interfaces.Commands;

public interface IDiscountCommandService
{
    Task<DiscountResponseDto> AddDiscountAsync(CreateDiscountDto createDiscountDto);    
    Task DeleteDiscountByIdAsync(int discountId);   
}