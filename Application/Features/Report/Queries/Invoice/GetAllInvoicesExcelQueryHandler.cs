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
        private readonly IDepartmentService _departmentService; // ✅ أضفناها
        private readonly ISystemInfoService _systemInfoService;

        public GetAllInvoicesExcelQueryHandler(
            IMeterReadingService meterReadingService,
            IElectricityReportService excelService,
            IDepartmentService departmentService, // ✅ حقن جديد
            ISystemInfoService systemInfoService)
        {
            _meterReadingService = meterReadingService;
            _excelService = excelService;
            _departmentService = departmentService;
            _systemInfoService = systemInfoService;
        }

        public async Task<byte[]> Handle(GetAllInvoicesExcelQuery request, CancellationToken cancellationToken)
        {
            var systemInfos = await _systemInfoService.GetAllAsync();
            var systemInfo = systemInfos.FirstOrDefault();

            // 1. جلب القراءات الموجودة للشهر والسنة
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);
            // ToLookup مو ToDictionary: القسم ممكن يكون إله أكثر من قراءة بنفس الشهر (قراءة إضافية مؤكَّدة عن قصد)،
            // وبهالحالة لازم تطلع فاتورة لكل قراءة مو تصير استثناء
            var readingsLookup = readings.ToLookup(r => r.DepartmentId);

            // 2. جلب كل الأقسام
            var allDepartments = await _departmentService.GetAllAsync();

            var invoicesList = new List<ElectricityReportResponseDto>();
            int index = 1;

            foreach (var dept in allDepartments)
            {
                // نفس منطق تقرير الاستهلاك: القسم غير الفعال ما إله قراءة ولا فاتورة بالمرة
                if (!dept.IsActive)
                    continue;

                // القسم ذو الاشتراك الثابت ما إله قراءة، بس لازم تطلع له فاتورة بقيمة الاستهلاك الثابتة
                if (dept.IsFixed)
                {
                    invoicesList.Add(new ElectricityReportResponseDto
                    {
                        Number = index++,
                        DepartmentName = dept.Name,
                        PreviousReading = 0,
                        CurrentReading = 0,
                        ActualConsumption = dept.FixedValue,
                        ConversionFactor = dept.ConversionFactor,
                        PricePerKilo = systemInfo.ElectricityPricePerKwh,
                        TotalCost = dept.FixedValue * systemInfo.ElectricityPricePerKwh * dept.ConversionFactor,
                        IsMissing = false
                    });
                    continue;
                }

                var deptReadings = readingsLookup[dept.Id];
                if (deptReadings.Any())
                {
                    foreach (var reading in deptReadings)
                    {
                        invoicesList.Add(new ElectricityReportResponseDto
                        {
                            Number = index++,
                            DepartmentName = !string.IsNullOrEmpty(reading.DepartmentName) ? reading.DepartmentName : (reading.Department?.Name ?? dept.Name),
                            PreviousReading = reading.PreviousReading,
                            CurrentReading = reading.CurrentReading,
                            ActualConsumption = reading.ActualConsumption,
                            ConversionFactor = reading.Department?.ConversionFactor ?? 1,
                            PricePerKilo = reading.PricePerUnit,
                            TotalCost = reading.TotalCost,
                            IsMissing = false
                        });
                    }
                }
                else
                {
                    // ما فيه قراءة -> نضيف صف أحمر
                    invoicesList.Add(new ElectricityReportResponseDto
                    {
                        Number = index++,
                        DepartmentName = dept.Name + " (لم تقرأ بعد)",
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

            return _excelService.GenerateAllInvoicesExcel(invoicesList, request.Year, (int)request.Month);
        }
    }
}