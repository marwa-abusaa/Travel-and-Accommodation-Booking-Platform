using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(h => h.HotelId);

        builder.Property(h => h.Name).IsRequired().HasMaxLength(50);
        builder.Property(h => h.Location).IsRequired().HasMaxLength(50);
        builder.Property(h => h.FullDescription).IsRequired().HasMaxLength(400);
        builder.Property(h => h.PhoneNumber).IsRequired();

        builder.Property(h => h.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(h => h.City)
               .WithMany(c => c.Hotels)
               .HasForeignKey(h => h.CityId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne(h => h.Owner)
               .WithMany(o => o.Hotels)
               .HasForeignKey(h => h.OwnerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

        builder.HasOne(h => h.Thumbnail)
               .WithMany()
               .HasForeignKey(h => h.ThumbnailId)
               .OnDelete(DeleteBehavior.SetNull);

    }
}