using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.MeterReading
{
    public record UpdateMeterReadingRequestDto(
        Months? Month,
        decimal? PreviousReading,
        decimal? CurrentReading
    );
}
