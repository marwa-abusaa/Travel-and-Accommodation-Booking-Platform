using FluentValidation;
using TravelAndAccommodationBookingPlatform.WebAPI.Dtos.Authentication;

namespace TravelAndAccommodationBookingPlatform.WebAPI.Validators.Authentication;

public class LogInRequestDtoValidator : AbstractValidator<LogInRequestDto>
{
    public LogInRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}
