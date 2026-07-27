using Application_Contract.DTOs.Department;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application_Contract.DTOs.MeterReading
{
    public class MeterReadingResponseDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentResponseDto? Department { get; set; }
        public Months Month { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal ActualConsumption { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public decimal PricePerUnit { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
