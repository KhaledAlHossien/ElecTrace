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
        private readonly ISystemInfoService _systemInfoService;
        private readonly IElectricityReportService _excelService;

        public GetElectricityExcelReportQueryHandler(
            IMeterReadingService meterReadingService,
            ISystemInfoService systemInfoService,
            IElectricityReportService excelService)
        {
            _meterReadingService = meterReadingService;
            _systemInfoService = systemInfoService;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(GetElectricityExcelReportQuery request, CancellationToken cancellationToken)
        {
            var systemInfoList = await _systemInfoService.GetAllAsync();
            var systemInfo = systemInfoList?.FirstOrDefault();

            decimal price = systemInfo?.ElectricityPricePerKwh ?? 2.50m;

            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

            if (readings == null || !readings.Any())
            {
                return Array.Empty<byte>();
            }

            int currentMonthNumber = (int)request.Month;
            int previousMonthNumber = currentMonthNumber == 1 ? 12 : currentMonthNumber - 1;

            string excelHeaderTitle = $"جدول استهلاك الكهرباء للسنتر لفعاليات عن شهر {currentMonthNumber} / لعام {request.Year}";

            int index = 1;
            var reportData = readings.Select(m => new ElectricityReportResponseDto
            {
                Number = index++,
                // ⚠️ تعديل: استخدام الـ Snapshot المخزن (DepartmentName) لضمان دقة التقرير التاريخي
                DepartmentName = !string.IsNullOrEmpty(m.DepartmentName) ? m.DepartmentName : (m.Department?.Name ?? "قسم غير معروف"),

                PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
                CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",
                PreviousReading = m.PreviousReading,
                CurrentReading = m.CurrentReading,
                ActualConsumption = m.ActualConsumption,

                ConversionFactor = m.Department?.ConversionFactor ?? 1,
                PricePerKilo = price,

                // ⚠️ إضافة التكلفة الكلية الصافية المخزنة في القراءة
                TotalCost = m.TotalCost
            }).ToList();

            return _excelService.GenerateElectricityExcelReport(reportData, excelHeaderTitle);
        }
    }
}