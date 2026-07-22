using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Features.MeterReading.Command.Cerate
{
    public class CreateMeterReadingCommandHandler : IRequestHandler<CreateMeterReadingCommand, MeterReadingResponseDto>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IDepartmentService _departmentService;
        private readonly ISystemInfoService _systemInfoService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateMeterReadingCommandHandler(
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

        public async Task<MeterReadingResponseDto> Handle(CreateMeterReadingCommand request, CancellationToken cancellationToken)
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

            await _meterReadingService.AddAsync(meterReading);

            return _mapper.Map<MeterReadingResponseDto>(meterReading);
        }
    }
}