using FluentValidation;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Invoices;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Invoices;

public class InvoiceCreationRequestDtoValidator : AbstractValidator<InvoiceCreationRequestDto>
{
    public InvoiceCreationRequestDtoValidator()
    {
        RuleFor(x => x.InvoiceDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("InvoiceDate cannot be in the future.");

        RuleFor(x => x.PaymentStatus)
            .IsInEnum().WithMessage("Invalid payment status.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
