using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.SystemInfo
{
    public record SystemInfoResponseDto(int Id, decimal ElectricityPricePerKwh);
}
