using Application.Features.Report.Queries.Invoice;
using Application.Features.Reports.Queries.GetAllInvoicesExcel;
using Application_Contract.DTOs.Report;
using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Reports.Queries.GetAllInvoicesExcel
{
    public class GetAllInvoicesExcelQueryHandler : IRequestHandler<GetAllInvoicesExcelQuery, byte[]>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly ISystemInfoService _systemInfoService;
        private readonly IElectricityReportService _excelService;

        public GetAllInvoicesExcelQueryHandler(
            IMeterReadingService meterReadingService,
            ISystemInfoService systemInfoService,
            IElectricityReportService excelService)
        {
            _meterReadingService = meterReadingService;
            _systemInfoService = systemInfoService;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(GetAllInvoicesExcelQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب سعر النظام الاحتياطي في حال الحاجة لعرض السعر في حقول أخرى بالتقرير
            var systemInfoList = await _systemInfoService.GetAllAsync();
            decimal price = systemInfoList?.FirstOrDefault()?.ElectricityPricePerKwh ?? 2.50m;

            // 2. جلب قراءات العدادات الفعالة للشهر والسنة المحددين
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

            if (readings == null || !readings.Any())
            {
                return Array.Empty<byte>();
            }

            // 3. تحويل القراءات إلى الـ DTO الخاص بالتقرير بالاعتماد على الـ Snapshot المخزن بالداتابيز
            var invoicesList = readings.Select((m, index) => new ElectricityReportResponseDto
            {
                Number = index + 1, // توليد رقم تسلسلي ديناميكي لكل سطر بالجدول

                // ⚠️ أخذ اسم القسم المخزن تاريخياً بجدول القراءات، والرجوع للـ Department كـ fallback فقط
                DepartmentName = !string.IsNullOrEmpty(m.DepartmentName) ? m.DepartmentName : (m.Department?.Name ?? "قسم غير معروف"),

                // جلب القراءات السابقة والحالية لعرضها بجدول التقرير العام إذا لزم الأمر
                PreviousReading = m.PreviousReading,
                CurrentReading = m.CurrentReading,
                ActualConsumption = m.ActualConsumption,

                // عامل الضرب نأخذه من كائن القسم الفعال حالياً
                ConversionFactor = m.Department?.ConversionFactor ?? 1,
                PricePerKilo = price,

                // ⚠️ التعديل الجوهري: ربط السعر الإجمالي الصافي المخزن بالقراءات مباشرة بالتقرير
                TotalCost = m.TotalCost
            }).ToList();

            // 4. توليد ملف الـ Excel وإرجاعه كـ byte array
            return _excelService.GenerateAllInvoicesExcel(invoicesList, request.Year, (int)request.Month);
        }
    }
}