using Application.Features.MeterReading.Command.ImportReadings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.ImportCurrentReadings
{
  internal class ImportCurrentReadingsValidator : AbstractValidator<ImportCurrentReadingsCommand>
  {
    public ImportCurrentReadingsValidator()
    {
      RuleFor(x => x.FileStream)
              .NotNull().WithMessage("Please select a file.");

      RuleFor(x => x.Month)
          .IsInEnum().WithMessage("Please select a valid month.");

      RuleFor(x => x.Year)
          .InclusiveBetween(2000, 2099).WithMessage("Invalid year. Please enter a year between 2000 and 2099.");
    }
  }
}
