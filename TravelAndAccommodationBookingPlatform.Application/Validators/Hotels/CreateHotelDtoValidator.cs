using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Hotels;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Hotels;

public class CreateHotelDtoValidator : AbstractValidator<CreateHotelDto>
{
    public CreateHotelDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hotel name is required.")
            .MaximumLength(200).WithMessage("Hotel name must not exceed 200 characters.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("CityId is required.")
            .GreaterThan(0).WithMessage("CityId must be greater than 0.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.")
            .GreaterThan(0).WithMessage("OwnerId must be greater than 0.");

        RuleFor(x => x.FullDescription)
            .NotEmpty().WithMessage("Full description is required.")
            .MaximumLength(2000).WithMessage("Full description must not exceed 2000 characters.");

        RuleFor(x => x.BriefDescription)
            .NotEmpty().WithMessage("Brief description is required.")
            .MaximumLength(500).WithMessage("Brief description must not exceed 500 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Phone number is invalid.");

        RuleFor(x => x.StarRating)
            .InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(300).WithMessage("Location must not exceed 300 characters.");
    }
}
