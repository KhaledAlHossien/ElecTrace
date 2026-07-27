using Application.Features.MeterReading.Command.Cerate;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.AddFromMobile
{
  internal class AddMeterReadingFromMobileCommandValidator
: AbstractValidator<AddMeterReadingFromMobileCommand>
  {
    public AddMeterReadingFromMobileCommandValidator()
    {
      RuleFor(x => x.Dto.MeterCode)
          .NotEmpty().WithMessage("Meter Code is required.");

      RuleFor(x => x.Dto.Month)
          .IsInEnum().WithMessage("Invalid month value.");

      RuleFor(x => x.Dto.CurrentReading)
          .GreaterThanOrEqualTo(0).WithMessage("Current reading cannot be negative.");
    }
  }
}
