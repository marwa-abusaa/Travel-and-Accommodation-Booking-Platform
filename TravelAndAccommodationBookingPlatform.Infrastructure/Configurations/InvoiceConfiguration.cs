using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TravelAndAccommodationBookingPlatform.Core.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(iv => iv.InvoiceId);

        builder.Property(iv => iv.InvoiceDate).IsRequired();
        builder.Property(iv => iv.PaymentStatus).IsRequired();
        builder.Property(iv => iv.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();


        builder.HasOne(iv => iv.Booking)
       .WithOne(b => b.Invoice) 
       .HasForeignKey<Invoice>(iv => iv.BookingId)
       .OnDelete(DeleteBehavior.Restrict);
    }
}