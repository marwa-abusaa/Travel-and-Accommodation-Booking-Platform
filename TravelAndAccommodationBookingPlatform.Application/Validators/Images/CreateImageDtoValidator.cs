using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Images;

public class CreateImageDtoValidator : AbstractValidator<CreateImageDto>
{
    public CreateImageDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Image name is required.")
            .MaximumLength(200).WithMessage("Image name must not exceed 200 characters.");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Image path is required.")
            .MaximumLength(1000).WithMessage("Image path must not exceed 1000 characters.");

        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("HotelId path is required.")
            .GreaterThan(0).WithMessage("HotelId must be greater than 0.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid image type.");

        When(x => x.RoomId.HasValue, () =>
        {
            RuleFor(x => x.RoomId.Value)
                .GreaterThan(0).WithMessage("RoomId must be greater than 0.");
        });
    }
}
