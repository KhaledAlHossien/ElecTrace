using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.Cerate
{
    public class CreateMeterReadingCommandValidator : AbstractValidator<CreateMeterReadingCommand>
    {
        public CreateMeterReadingCommandValidator()
        {
            RuleFor(x => x.Dto.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.")
                .GreaterThan(0).WithMessage("Department ID must be greater than 0.");

            RuleFor(x => x.Dto.Month)
                .IsInEnum().WithMessage("Invalid month value.");

            RuleFor(x => x.Dto.CurrentReading)
                .GreaterThanOrEqualTo(0).WithMessage("Current reading cannot be negative.");
        }
    }
}
