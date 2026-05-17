using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.MeterReading
{
    public record MeterReadingResponseDto(
        int Id,
        int DepartmentId,

        Months Month,
        decimal PreviousReading,
        decimal CurrentReading,
        decimal ActualConsumption,
        DateTime CreatedAt
    );
}
