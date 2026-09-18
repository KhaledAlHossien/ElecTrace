using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SystemInfo
    {
        public int Id { get; set; }
        public decimal ElectricityPricePerKwh { get; set; }

        // سلسلة الاتصال بقاعدة بيانات نظام الأمين (تقارير الحركة اليومية)
        public string? AmeenConnectionString { get; set; }
    }
}
