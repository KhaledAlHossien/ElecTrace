using Application.Features.Report.Queries.Invoice;
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
        private readonly IElectricityReportService _excelService;

        public GetAllInvoicesExcelQueryHandler(
            IMeterReadingService meterReadingService,
            IElectricityReportService excelService)
        {
            _meterReadingService = meterReadingService;
            _excelService = excelService;
        }

        public async Task<byte[]> Handle(GetAllInvoicesExcelQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب قراءات العدادات الفعالة للشهر والسنة المحددين
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

            if (readings == null || !readings.Any())
            {
                return Array.Empty<byte>();
            }

            // 2. تحويل القراءات إلى الـ DTO الخاص بالتقرير
            var invoicesList = readings.Select((m, index) => new ElectricityReportResponseDto
            {
                Number = index + 1, // رقم تسلسلي ديناميكي

                // استخدام الـ Snapshot المخزن (DepartmentName) لضمان دقة التقرير التاريخي
                DepartmentName = !string.IsNullOrEmpty(m.DepartmentName) ? m.DepartmentName : (m.Department?.Name ?? "قسم غير معروف"),

                PreviousReading = m.PreviousReading,
                CurrentReading = m.CurrentReading,
                ActualConsumption = m.ActualConsumption,

                // عامل الضرب نأخذه من كائن القسم
                ConversionFactor = m.Department?.ConversionFactor ?? 1,

                // ⚠️ الاعتماد الكلي على السعر التاريخي المخزن في جدول القراءات
                // هذا يضمن أن التقرير يعرض "الفاتورة" كما صدرت فعلياً في ذلك الوقت
                PricePerKilo = m.PricePerUnit,

                // ⚠️ التكلفة الكلية الصافية المخزنة سابقاً في القراءة
                TotalCost = m.TotalCost
            }).ToList();

            // 3. توليد ملف الـ Excel وإرجاعه
            return _excelService.GenerateAllInvoicesExcel(invoicesList, request.Year, (int)request.Month);
        }
    }
}