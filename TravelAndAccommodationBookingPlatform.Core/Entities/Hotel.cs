namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class Hotel : AuditableEntity
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CityId { get; set; }
    public City City { get; set; }
    public int OwnerId { get; set; }
    public Owner Owner { get; set; }
    public int? ThumbnailId { get; set; }
    public Image? Thumbnail { get; set; }
    public string FullDescription { get; set; } = string.Empty;
    public string BriefDescription { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string Location { get; set; } = string.Empty;
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
