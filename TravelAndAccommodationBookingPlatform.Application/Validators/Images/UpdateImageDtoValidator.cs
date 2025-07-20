using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Images;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Images;

public class UpdateImageDtoValidator : AbstractValidator<UpdateImageDto>
{
    public UpdateImageDtoValidator()
    {
        RuleFor(x => x.ImageId)
            .NotEmpty().WithMessage("ImageId is required.")
            .GreaterThan(0).WithMessage("ImageId must be greater than 0.");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Image name must not exceed 200 characters.");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Image path is required.")
            .MaximumLength(1000).WithMessage("Image path must not exceed 1000 characters.");

        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("HotelId path is required.")
            .GreaterThan(0).WithMessage("HotelId must be greater than 0.");

        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("RoomId must be greater than 0.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid image type.");
    }
}
