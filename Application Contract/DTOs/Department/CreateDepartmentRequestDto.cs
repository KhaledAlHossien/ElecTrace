using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Department
{
    public record CreateDepartmentRequestDto(string Name,decimal ConversionFactor,string MeterCode, decimal Discount = 0 , bool IsActive = true, bool IsFixed = false, decimal FixedValue = 0);
}
