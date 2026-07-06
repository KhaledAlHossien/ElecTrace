using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Update
{
    public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid Department ID.");

            RuleFor(x => x.DeptDto.Name)
                .MaximumLength(100)
                .When(x => x.DeptDto.Name != null);

            RuleFor(x => x.DeptDto.MeterCode)
                .MinimumLength(2)
                .When(x => x.DeptDto.MeterCode != null);

            RuleFor(x => x.DeptDto.ConversionFactor)
                .GreaterThan(0)
                .When(x => x.DeptDto.ConversionFactor.HasValue);
        }
    }
}
