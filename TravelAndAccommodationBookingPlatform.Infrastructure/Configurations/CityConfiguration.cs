using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.CityId);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Country).IsRequired().HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");
    }
}