using FluentValidation;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Discounts;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Discounts;

public class DiscountCreationRequestDtoValidator : AbstractValidator<DiscountCreationRequestDto>
{
    public DiscountCreationRequestDtoValidator()
    {
        RuleFor(x => x.Percentage)
            .GreaterThan(0).WithMessage("Percentage must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Percentage cannot exceed 100.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.");
    }
}

