using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal ConversionFactor { get; set; }

        public string MeterCode { get; set; } = string.Empty;

        public double MaxCounter { get; set; }

        public decimal Discount { get; set; } = 0;
    }
}
