using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => d.DiscountId);

        builder.Property(d => d.StartDate).IsRequired();
        builder.Property(d => d.EndDate).IsRequired();
        builder.Property(d => d.Percentage).HasColumnType("decimal(18,2)");

        builder.Property(d => d.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(d => d.Room)
               .WithMany(rm => rm.Discounts)
               .HasForeignKey(d => d.RoomId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}