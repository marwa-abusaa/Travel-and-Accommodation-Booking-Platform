using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Cities;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Cities;

public class CreateCityDtoValidator : AbstractValidator<CreateCityDto>
{
    public CreateCityDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100).WithMessage("City name must be at most 100 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country name is required.")
            .MaximumLength(100).WithMessage("Country name must be at most 100 characters.");

        RuleFor(x => x.PostOffice)
            .NotEmpty().WithMessage("Post office is required.")
            .MaximumLength(50).WithMessage("Post office must be at most 50 characters.");
    }
}

