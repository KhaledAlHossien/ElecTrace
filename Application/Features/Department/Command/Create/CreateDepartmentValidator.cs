using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Create
{
    public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.DeptDto.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DeptDto.ConversionFactor).GreaterThan(0);
        }
    }
}
