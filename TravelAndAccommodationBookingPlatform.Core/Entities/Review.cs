namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Review : AuditableEntity
{
    public int ReviewId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Rate { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
