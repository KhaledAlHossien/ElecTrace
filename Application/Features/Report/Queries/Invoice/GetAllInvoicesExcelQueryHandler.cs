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

        public GetAllInvoicesExcelQueryHandler(
            IMeterReadingService meterReadingService,
            IElectricityReportService excelService,
            IDepartmentService departmentService) // ✅ حقن جديد
        {
            _meterReadingService = meterReadingService;
            _excelService = excelService;
            _departmentService = departmentService;
        }

        public async Task<byte[]> Handle(GetAllInvoicesExcelQuery request, CancellationToken cancellationToken)
        {
            // 1. جلب القراءات الموجودة للشهر والسنة
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);
            var readingsDict = readings.ToDictionary(r => r.DepartmentId);

            // 2. جلب كل الأقسام
            var allDepartments = await _departmentService.GetAllAsync();

            var invoicesList = new List<ElectricityReportResponseDto>();
            int index = 1;

            foreach (var dept in allDepartments)
            {
                if (readingsDict.TryGetValue(dept.Id, out var reading))
                {
                    // فيه قراءة
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