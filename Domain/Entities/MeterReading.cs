using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class MeterReading
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string DepartmentName { get; set; } = string.Empty;


        public Months Month { get; set; }

        public decimal PreviousReading { get; set; }

        public decimal CurrentReading { get; set; }

        public decimal ActualConsumption { get; private set; }
        public decimal TotalCost { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ميثود ذكية لحساب الاستهلاك الفعلي مع معالجة التفاف العداد (Rollover)
        /// </summary>
        /// <param name="maxCounter">الحد الأقصى للعداد القادم من جدول الـ Department</param>
        public void CalculateConsumption(double maxCounter)
        {
            decimal maxAsDecimal = (decimal)maxCounter;
            decimal difference = CurrentReading - PreviousReading;

            if (difference < 0)
            {
                ActualConsumption = difference + maxAsDecimal;
            }
            else
            {
                ActualConsumption = difference;
            }
        }
        public void CalculateTotalCost(decimal pricePerKwh, decimal departmentDiscount)
        {
            decimal costBeforeDiscount = ActualConsumption * pricePerKwh;
            decimal costAfterDiscount = costBeforeDiscount - departmentDiscount;

            // حماية لكي لا تصبح الفاتورة بالسالب إذا كان الخصم أكبر من الاستهلاك
            TotalCost = costAfterDiscount < 0 ? 0 : costAfterDiscount;
        }
    }
}

