using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Department
{
    public record DepartmentResponseDto(int Id,string Name,decimal ConversionFactor,string MeterCode,double MaxCounter,decimal Discount);
}
