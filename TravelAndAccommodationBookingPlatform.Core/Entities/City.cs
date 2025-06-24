namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public class City
{
    public int CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
