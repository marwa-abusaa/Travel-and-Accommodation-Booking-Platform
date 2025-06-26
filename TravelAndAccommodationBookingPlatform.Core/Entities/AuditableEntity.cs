namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public abstract class AuditableEntity
{
    public DateTime? UpdatedAt { get; set; }
}
