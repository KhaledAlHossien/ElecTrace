using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Text;
namespace Application_Contract.Interfaces
{
    public interface IElectricityReportService
    {
        byte[] GenerateElectricityExcelReport(IEnumerable<ElectricityReportResponseDto> reportData, string title);
        byte[] GenerateAllInvoicesExcel(IEnumerable<ElectricityReportResponseDto> allInvoicesData, int year, int month);

        byte[] GenerateDepartmentsExcel(IEnumerable<MeterCodeNameDto> departments, string sheetName = "الأقسام");


    }
}
