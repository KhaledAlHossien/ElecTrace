using Application_Contract.DTOs.Report;
using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Report.Queries.GetElectricityExcelReport
{
    public class GetElectricityExcelReportQueryHandler : IRequestHandler<GetElectricityExcelReportQuery, byte[]>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IElectricityReportService _excelService;

        // لا نحتاج لـ ISystemInfoService هنا لأننا نعتمد على البيانات التاريخية المخزنة في القراءة نفسها
        public GetElectricityExcelReportQueryHandler(
            IMeterReadingService meterReadingService,
            IElectricityReportService excelService)
        {
            _meterReadingService = meterReadingService;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(GetElectricityExcelReportQuery request, CancellationToken cancellationToken)
        {
            // جلب القراءات الخاصة بالشهر والسنة المطلوبة
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

            if (readings == null || !readings.Any())
            {
                return Array.Empty<byte>();
            }

            int currentMonthNumber = (int)request.Month;
            int previousMonthNumber = currentMonthNumber == 1 ? 12 : currentMonthNumber - 1;

            string excelHeaderTitle = $"جدول استهلاك الكهرباء للسنتر لفعاليات عن شهر {currentMonthNumber} / لعام {request.Year}";

            int index = 1;

            // تحويل القراءات إلى DTO الخاص بالتقرير
            var reportData = readings.Select(m => new ElectricityReportResponseDto
            {
                Number = index++,

                // استخدام الـ Snapshot المخزن (DepartmentName) لضمان دقة التقرير التاريخي
                DepartmentName = !string.IsNullOrEmpty(m.DepartmentName) ? m.DepartmentName : (m.Department?.Name ?? "قسم غير معروف"),

                PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
                CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",

                PreviousReading = m.PreviousReading,
                CurrentReading = m.CurrentReading,
                ActualConsumption = m.ActualConsumption,

                ConversionFactor = m.Department?.ConversionFactor ?? 1,

                // ⚠️ الاعتماد على السعر المخزن داخل سجل القراءة (PricePerUnit)
                // هذا يضمن أن التقرير يعرض السعر الذي تمت المحاسبة بناءً عليه فعلياً في ذلك الوقت
                PricePerKilo = m.PricePerUnit,

                // التكلفة الكلية الصافية المخزنة في القراءة
                TotalCost = m.TotalCost
            }).ToList();

            return _excelService.GenerateElectricityExcelReport(reportData, excelHeaderTitle);
        }
    }
}