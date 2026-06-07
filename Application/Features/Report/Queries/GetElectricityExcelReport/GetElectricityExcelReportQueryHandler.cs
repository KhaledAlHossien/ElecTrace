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
        private readonly IDepartmentService _departmentService;

        public GetElectricityExcelReportQueryHandler(
            IMeterReadingService meterReadingService,
            IElectricityReportService excelService,
            IDepartmentService departmentService)   // حقن جديد
        {
            _meterReadingService = meterReadingService;
            _excelService = excelService;
            _departmentService = departmentService;
        }

        public async Task<byte[]> Handle(GetElectricityExcelReportQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب القراءات الخاصة بالشهر والسنة
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);
            var readingsDict = readings.ToDictionary(r => r.DepartmentId);

            // 2. جلب جميع الأقسام (كل الأقسام المسجلة)
            var allDepartments = await _departmentService.GetAllAsync();

            int currentMonthNumber = (int)request.Month;
            int previousMonthNumber = currentMonthNumber == 1 ? 12 : currentMonthNumber - 1;

            string excelHeaderTitle = $"جدول استهلاك الكهرباء للسنتر لفعاليات عن شهر {currentMonthNumber} / لعام {request.Year}";

            int index = 1;
            var reportData = new List<ElectricityReportResponseDto>();

            foreach (var dept in allDepartments)
            {
                if (readingsDict.TryGetValue(dept.Id, out var reading))
                {
                    // يوجد قراءة لهذا القسم في الشهر المطلوب
                    reportData.Add(new ElectricityReportResponseDto
                    {
                        Number = index++,
                        DepartmentName = !string.IsNullOrEmpty(reading.DepartmentName)
                                         ? reading.DepartmentName
                                         : (reading.Department?.Name ?? dept.Name),
                        PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
                        CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",
                        PreviousReading = reading.PreviousReading,
                        CurrentReading = reading.CurrentReading,
                        ActualConsumption = reading.ActualConsumption,
                        ConversionFactor = reading.Department?.ConversionFactor ?? 1,
                        PricePerKilo = reading.PricePerUnit,
                        TotalCost = reading.TotalCost,
                        IsMissing = false
                    });
                }
                else
                {
                    // لا توجد قراءة لهذا القسم -> نضيف صفاً أحمر بقيم صفرية
                    reportData.Add(new ElectricityReportResponseDto
                    {
                        Number = index++,
                        DepartmentName = dept.Name + " (لم تقرأ بعد)",
                        PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
                        CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",
                        PreviousReading = 0,
                        CurrentReading = 0,
                        ActualConsumption = 0,
                        ConversionFactor = dept.ConversionFactor,
                        PricePerKilo = 0,
                        TotalCost = 0,
                        IsMissing = true
                    });
                }
            }

            return _excelService.GenerateElectricityExcelReport(reportData, excelHeaderTitle);
        }
    }
}