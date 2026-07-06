using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Department
{
    public record UpdateDepartmentRequestDto(string? Name, decimal? ConversionFactor, string? MeterCode, decimal? Discount = 0);

}
