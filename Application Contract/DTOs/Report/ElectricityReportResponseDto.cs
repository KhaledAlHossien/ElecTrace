using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.Report
{
    public class ElectricityReportResponseDto
    {
        public int Number { get; set; } 
        public string DepartmentName { get; set; } 

        public string? PreviousMonthLabel { get; set; }
        public string? CurrentMonthLabel { get; set; }

        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; } 
        public decimal ActualConsumption { get; set; } 
        public decimal ConversionFactor { get; set; } 
        public decimal PricePerKilo { get; set; } 

        public decimal TotalCost => ActualConsumption * (decimal)ConversionFactor * PricePerKilo;
    }
}
