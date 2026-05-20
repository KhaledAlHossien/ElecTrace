using Application.Features.Report.Queries.Invoice;
using Application.Features.Reports.Queries.GetAllInvoicesExcel;
using Application_Contract.DTOs.Report;
using Application_Contract.Interfaces;
using MediatR;
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
            var systemInfoList = await _systemInfoService.GetAllAsync();
            decimal price = systemInfoList?.FirstOrDefault()?.ElectricityPricePerKwh ?? 2.50m;

            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

            if (readings == null || !readings.Any())
            {
                return Array.Empty<byte>();
            }

            var invoicesList = readings.Select(m => new ElectricityReportResponseDto
            {
                Number = 1,
                DepartmentName = m.Department?.Name ?? "قسم غير معروف",
                ActualConsumption = m.ActualConsumption,
                ConversionFactor = m.Department?.ConversionFactor ?? 1,
                PricePerKilo = price
            }).ToList();

            return _excelService.GenerateAllInvoicesExcel(invoicesList, request.Year, (int)request.Month);
        }
    }
}
