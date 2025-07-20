using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(rv => rv.ReviewId);

        builder.Property(rv => rv.Rate).IsRequired();
        builder.Property(rv => rv.Comment).HasMaxLength(200);

        builder.Property(rv => rv.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(rv => rv.User)
               .WithMany(u => u.Reviews)
               .HasForeignKey(rv => rv.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasOne(rv => rv.Hotel)
               .WithMany(h => h.Reviews)
               .HasForeignKey(rv => rv.HotelId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
    }
}