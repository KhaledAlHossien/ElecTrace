using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.MeterReading
{
    public record CreateMeterReadingRequestDto(
        int DepartmentId,
        Months Month,
        decimal CurrentReading
    );
}
