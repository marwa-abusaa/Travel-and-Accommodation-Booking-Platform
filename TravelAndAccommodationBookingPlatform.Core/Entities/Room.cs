using TravelAndAccommodationBookingPlatform.Core.Enums;

namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Room
{
    public int RoomId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int PricePerNight { get; set; }
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public RoomClass RoomClass  { get; set; }
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
