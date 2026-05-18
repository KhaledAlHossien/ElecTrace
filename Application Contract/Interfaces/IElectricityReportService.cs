using System;
using System.Collections.Generic;
using System.Text;
using Application_Contract.DTOs.Report;
namespace Application_Contract.Interfaces
{
    public interface IElectricityReportService
    {
        byte[] GenerateElectricityExcelReport(IEnumerable<ElectricityReportResponseDto> reportData, string title);
        byte[] GenerateAllInvoicesExcel(IEnumerable<ElectricityReportResponseDto> allInvoicesData, int year, int month);
    }
}
