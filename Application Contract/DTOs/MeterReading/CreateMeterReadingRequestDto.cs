using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.MeterReading
{
    public record CreateMeterReadingRequestDto(
        string MeterCode,
        Months Month,
        decimal CurrentReading,
        // true = المستخدم أكّد الاستمرار رغم وجود قراءة سابقة لنفس القسم بنفس الشهر
        bool ForceDuplicate = false
    );
}
