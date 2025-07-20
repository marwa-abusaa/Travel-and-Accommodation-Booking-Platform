using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(im => im.ImageId);

        builder.Property(im => im.Name).HasMaxLength(50).IsRequired();
        builder.Property(im => im.Path).HasMaxLength(200).IsRequired();


        builder.HasOne(im => im.Room)
               .WithMany(rm => rm.Images)
               .HasForeignKey(im => im.RoomId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(im => im.Hotel)
               .WithMany(h => h.Images)
               .HasForeignKey(im => im.HotelId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}