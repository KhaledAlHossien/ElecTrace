using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Department
{
    public record DepartmentResponseDto(int Id,string Name,decimal ConversionFactor,string MeterCode,decimal Discount,bool IsActive,bool IsFixed,decimal FixedValue);
}
