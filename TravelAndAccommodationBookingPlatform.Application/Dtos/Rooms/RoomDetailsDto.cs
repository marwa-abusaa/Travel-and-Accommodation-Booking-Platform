using TravelAndAccommodationBookingPlatform.Application.Dtos.Discounts;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;
using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

public class RoomDetailsDto
{
    public int RoomId { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public int HotelId { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public RoomClass RoomClass { get; set; }
    public ICollection<DiscountResponseDto> Discounts { get; set; } = new List<DiscountResponseDto>();
    public ICollection<ImageResponseDto> Images { get; set; } = new List<ImageResponseDto>();
}
