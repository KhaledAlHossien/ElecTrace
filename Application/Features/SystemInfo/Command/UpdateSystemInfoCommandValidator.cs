using Application.Features.SystemInfo.Command;
using FluentValidation;

namespace Application.Features.SystemInfo
{
    public class UpdateSystemInfoCommandValidator : AbstractValidator<UpdateSystemInfoCommand>
    {
        public UpdateSystemInfoCommandValidator()
        {
            RuleFor(x => x.Dto.ElectricityPricePerKwh)
                .NotNull().WithMessage("Electricity price per kWh is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Electricity price per kWh cannot be negative.");
        }
    }
}