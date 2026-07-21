/*
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
    private readonly ISystemInfoService _systemInfoService;

    public GetElectricityExcelReportQueryHandler(
        IMeterReadingService meterReadingService,
        IElectricityReportService excelService,
        IDepartmentService departmentService,
        ISystemInfoService systemInfoService)
    {
      _meterReadingService = meterReadingService;
      _excelService = excelService;
      _departmentService = departmentService;
      _systemInfoService = systemInfoService;
    }

    public async Task<byte[]> Handle(GetElectricityExcelReportQuery request, CancellationToken cancellationToken)
    {

      var systemInfos = await _systemInfoService.GetAllAsync();
      var systemInfo = systemInfos.FirstOrDefault();

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
        if (!dept.IsActive)
          continue;

        if(dept.IsFixed)
        {
          reportData.Add(new ElectricityReportResponseDto
          {
            Number = index++,
            DepartmentName = dept.Name,
            PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
            CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",
            PreviousReading = 0,
            CurrentReading = 0,
            ActualConsumption = dept.FixedValue,
            ConversionFactor = dept.ConversionFactor,
            PricePerKilo = systemInfo.ElectricityPricePerKwh,
            TotalCost = dept.FixedValue * systemInfo.ElectricityPricePerKwh * dept.ConversionFactor,
            IsMissing = false,
            IsRotated = false
          });
          continue;
        }

        if (readingsDict.TryGetValue(dept.Id, out var reading))
        {
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
            IsMissing = false,
            IsRotated = reading.CurrentReading - reading.PreviousReading < 0
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
            PricePerKilo = systemInfo.ElectricityPricePerKwh,
            TotalCost = 0,
            IsMissing = true,
            IsRotated = false
          });
        }
      }

      return _excelService.GenerateElectricityExcelReport(reportData, excelHeaderTitle);
    }
  }
}
*/

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
    private readonly ISystemInfoService _systemInfoService;

    public GetElectricityExcelReportQueryHandler(
        IMeterReadingService meterReadingService,
        IElectricityReportService excelService,
        IDepartmentService departmentService,
        ISystemInfoService systemInfoService)
    {
      _meterReadingService = meterReadingService;
      _excelService = excelService;
      _departmentService = departmentService;
      _systemInfoService = systemInfoService;
    }

    public async Task<byte[]> Handle(GetElectricityExcelReportQuery request, CancellationToken cancellationToken)
    {
      var systemInfos = await _systemInfoService.GetAllAsync();
      var systemInfo = systemInfos.FirstOrDefault();

      // 1. جلب القراءات الخاصة بالشهر والسنة
      var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

      // 🔥 تعديل مهم: السماح بوجود أكثر من قراءة لنفس القسم
      var readingsGrouped = readings
          .GroupBy(r => r.DepartmentId)
          .ToDictionary(g => g.Key, g => g.ToList());

      // 2. جلب جميع الأقسام
      var allDepartments = await _departmentService.GetAllAsync();

      int currentMonthNumber = (int)request.Month;
      int previousMonthNumber = currentMonthNumber == 1 ? 12 : currentMonthNumber - 1;

      string excelHeaderTitle =
          $"جدول استهلاك الكهرباء للسنتر لفعاليات عن شهر {currentMonthNumber} / لعام {request.Year}";

      int index = 1;
      var reportData = new List<ElectricityReportResponseDto>();

      foreach (var dept in allDepartments)
      {
        if (!dept.IsActive)
          continue;

        // الأقسام الثابتة
        if (dept.IsFixed)
        {
          reportData.Add(new ElectricityReportResponseDto
          {
            Number = index++,
            DepartmentName = dept.Name,
            PreviousMonthLabel = $"تأشيرة نهاية شهر {previousMonthNumber}",
            CurrentMonthLabel = $"تأشيرة نهاية شهر {currentMonthNumber}",
            PreviousReading = 0,
            CurrentReading = 0,
            ActualConsumption = dept.FixedValue,
            ConversionFactor = dept.ConversionFactor,
            PricePerKilo = systemInfo.ElectricityPricePerKwh,
            TotalCost = dept.FixedValue * systemInfo.ElectricityPricePerKwh * dept.ConversionFactor,
            IsMissing = false,
            IsRotated = false
          });

          continue;
        }

        // الأقسام ذات القراءات المتعددة
        if (readingsGrouped.TryGetValue(dept.Id, out var deptReadings))
        {
          foreach (var reading in deptReadings)
          {
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

              IsMissing = false,
              IsRotated = reading.CurrentReading - reading.PreviousReading < 0
            });
          }
        }
        else
        {
          // لا توجد قراءة لهذا القسم
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
            PricePerKilo = systemInfo.ElectricityPricePerKwh,
            TotalCost = 0,
            IsMissing = true,
            IsRotated = false
          });
        }
      }

      return _excelService.GenerateElectricityExcelReport(reportData, excelHeaderTitle);
    }
  }
}
