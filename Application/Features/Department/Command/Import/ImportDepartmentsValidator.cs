using Application.Features.MeterReading.Command.ImportReadings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Import
{
    public class ImportDepartmentsValidator : AbstractValidator<ImportDepartmentsCommand>
    {
        public ImportDepartmentsValidator()
        {
            RuleFor(x => x.FileStream)
                    .NotNull().WithMessage("Please select a file.");

        }
    }
}   
