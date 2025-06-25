using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(rm => rm.RoomId);
       
        builder.Property(rm => rm.Description).IsRequired().HasMaxLength(200);
        builder.Property(rm => rm.AdultCapacity).IsRequired();
        builder.Property(rm => rm.ChildrenCapacity).IsRequired();

        builder.HasIndex(rm => rm.RoomClass);

        builder.Property(rm => rm.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(rm => rm.Hotel)
              .WithMany(h => h.Rooms)
              .HasForeignKey(rm => rm.HotelId)
              .OnDelete(DeleteBehavior.Cascade)
              .IsRequired();
    }
}