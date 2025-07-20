using FluentValidation;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Bookings;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Bookings;

public class BookingCreationRequestDtoValidator : AbstractValidator<BookingCreationRequestDto>
{
    public BookingCreationRequestDtoValidator()
    {
        RuleFor(x => x.CheckInDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.ToLocalTime()))
            .WithMessage("Check-in date must be today or later.");

        RuleFor(x => x.CheckOutDate)
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("Check-out date must be after the check-in date.");

        RuleFor(x => x.RoomIds)
            .NotEmpty().WithMessage("At least one room must be selected.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Remarks));

        RuleFor(x => x.PaymentType)
            .IsInEnum().WithMessage("Invalid payment type selected.");
    }
}

