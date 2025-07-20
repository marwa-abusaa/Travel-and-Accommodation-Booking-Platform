using FluentValidation;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Reviews;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Reviews;

public class ReviewUpdateRequestDtoValidator : AbstractValidator<ReviewUpdateRequestDto>
{
    public ReviewUpdateRequestDtoValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");

        RuleFor(x => x.Rate)
            .InclusiveBetween(1, 5).WithMessage("Rate must be between 1 and 5.");
    }
}

