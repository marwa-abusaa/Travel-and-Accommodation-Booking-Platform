using FluentValidation;
using TravelAndAccommodationBookingPlatform.Application.Dtos.Rooms;

namespace TravelAndAccommodationBookingPlatform.Application.Validators.Rooms;

public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than zero.");

        RuleFor(x => x.HotelId)
            .GreaterThan(0).WithMessage("HotelId must be greater than 0.");

        RuleFor(x => x.AdultCapacity)
            .GreaterThanOrEqualTo(1).WithMessage("Adult capacity must be at least 1.");

        RuleFor(x => x.ChildrenCapacity)
            .GreaterThanOrEqualTo(0).WithMessage("Children capacity must be zero or more.");

        RuleFor(x => x.RoomClass)
            .IsInEnum().WithMessage("Invalid room class.");
    }
}
