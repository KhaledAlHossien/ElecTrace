using Application.Features.MeterReading.Command.Cerate;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application.Features.MeterReading.Command.AddFromMobile
{
  internal class AddMeterReadingFromMobileCommandHandler
: IRequestHandler<AddMeterReadingFromMobileCommand, MeterReadingResponseDto>
  {
    private readonly IMeterReadingService _meterReadingService;
    private readonly IDepartmentService _departmentService;
    private readonly ISystemInfoService _systemInfoService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddMeterReadingFromMobileCommandHandler(
        IMeterReadingService meterReadingService,
        IDepartmentService departmentService,
        ISystemInfoService systemInfoService,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
      _meterReadingService = meterReadingService;
      _departmentService = departmentService;
      _systemInfoService = systemInfoService;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MeterReadingResponseDto> Handle(AddMeterReadingFromMobileCommand request, CancellationToken cancellationToken)
    {
      var department = await _departmentService.GetByMeterCodeAsync(request.Dto.MeterCode);
      if (department == null)
      {
        throw new KeyNotFoundException($"Department with Meter Code {request.Dto.MeterCode} not found.");
      }
      if (!department.IsActive)
      {
        throw new KeyNotFoundException($"Department with Meter Code {request.Dto.MeterCode} is not active.");
      }
      if (department.IsFixed)
      {
        throw new KeyNotFoundException($"Department with Meter Code {request.Dto.MeterCode} is fixed.");
      }

      var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

      var systemInfos = await _systemInfoService.GetAllAsync();
      var systemInfo = systemInfos.FirstOrDefault();

      if (systemInfo == null)
      {
        throw new Exception("إعدادات النظام غير معرّفة، يرجى تحديد سعر الكيلو واط أولاً في النظام.");
      }


      decimal currentPricePerKwh = systemInfo.ElectricityPricePerKwh;

      var meterReading = _mapper.Map<Domain.Entities.MeterReading>(request.Dto);

      meterReading.DepartmentId = department.Id;
      meterReading.DepartmentName = department.Name;
      meterReading.CreatedByUserId = userId;

      var allDepartmentReadings = await _meterReadingService.GetByDepartmentIdAsync(department.Id);
      var lastReading = allDepartmentReadings.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

      meterReading.PreviousReading = lastReading?.CurrentReading ?? 0;

      meterReading.CalculateConsumption();

      meterReading.CalculateTotalCost(currentPricePerKwh, department.Discount, department.ConversionFactor);


      if (meterReading.ActualConsumption == 0)
      {
        if (request.Lang == Lang.Ar)
        { throw new InvalidOperationException("الاستهلاك الحالي لا يمكن أن يكون مساوياً للصفر."); }
        else
        { throw new InvalidOperationException("Current consumption cannot be zero."); }
      }

      if (meterReading.CurrentReading < meterReading.PreviousReading && !request.UnusualAccept)
      {
        if (request.Lang == Lang.Ar)
        { throw new InvalidOperationException("هل أنت متأكد من إدخال قيمة استهلاك أقل من القيمة الماضية"); }
        else
        { throw new InvalidOperationException("Are you sure you entered a consumption value lower than the previous one?"); }
      }

      await _meterReadingService.AddAsync(meterReading);

      return _mapper.Map<MeterReadingResponseDto>(meterReading);
    }
  }
}
