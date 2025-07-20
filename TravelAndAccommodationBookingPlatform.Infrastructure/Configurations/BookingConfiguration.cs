using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.BookingId);

        builder.Property(b => b.TotalPriceBeforeDiscount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(b => b.TotalPriceAfterDiscount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.PaymentType).IsRequired();
        builder.Property(b => b.CheckInDate).IsRequired();
        builder.Property(b => b.CheckOutDate).IsRequired();
        builder.Property(b => b.Remarks).HasMaxLength(400);


        builder.Property(b => b.BookingDate)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(b => b.User)
               .WithMany(u => u.Bookings)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();


        builder.HasMany(b => b.Rooms)
               .WithMany(rm => rm.Bookings)
               .UsingEntity<Dictionary<string, object>>("RoomBooking",
                    j => j.HasOne<Room>().WithMany().HasForeignKey("RoomId"),
                    j => j.HasOne<Booking>().WithMany().HasForeignKey("BookingId"));
    }
}