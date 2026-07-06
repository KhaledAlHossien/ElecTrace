using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Department
{
    public record CreateDepartmentRequestDto(string Name,decimal ConversionFactor, double MaxCounter = 0, decimal Discount = 0);
}
