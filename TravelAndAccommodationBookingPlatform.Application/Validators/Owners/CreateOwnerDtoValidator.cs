using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Owners;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Owners;

public class CreateOwnerDtoValidator : AbstractValidator<CreateOwnerDto>
{
    public CreateOwnerDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Phone number is invalid.");
    }
}
