using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasKey(o => o.OwnerId);

        builder.Property(o => o.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(o => o.LastName).IsRequired().HasMaxLength(50);
        builder.Property(o => o.PhoneNumber).IsRequired();

        builder.HasIndex(o => o.Email).IsUnique();
    }
}