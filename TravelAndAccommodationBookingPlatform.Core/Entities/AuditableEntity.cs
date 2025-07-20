namespace TravelAndAccommodationBookingPlatform.Core.Entities;

public abstract class AuditableEntity
{
    public virtual DateTime? UpdatedAt { get; set; }
}
