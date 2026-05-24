using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MeterReading.Command.Cerate
{
    public class CreateMeterReadingCommandHandler : IRequestHandler<CreateMeterReadingCommand, MeterReadingResponseDto>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IDepartmentService _departmentService;
        // ⚠️ حقن خدمة النظام لجلب سعر الكهرباء المخزن بـ SystemInfo
        private readonly ISystemInfoService _systemInfoService;
        private readonly IMapper _mapper;

        public CreateMeterReadingCommandHandler(
            IMeterReadingService meterReadingService,
            IDepartmentService departmentService,
            ISystemInfoService systemInfoService, // 👈 أضفناها هنا
            IMapper mapper)
        {
            _meterReadingService = meterReadingService;
            _departmentService = departmentService;
            _systemInfoService = systemInfoService; // 👈 أضفناها هنا
            _mapper = mapper;
        }

        public async Task<MeterReadingResponseDto> Handle(CreateMeterReadingCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentService.GetByIdAsync(request.Dto.DepartmentId);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {request.Dto.DepartmentId} not found.");
            }

            // جلب سعر الكيلو واط من SystemInfo
            var systemInfos = await _systemInfoService.GetAllAsync();
            var systemInfo = systemInfos.FirstOrDefault();

            if (systemInfo == null)
            {
                throw new Exception("إعدادات النظام غير معرّفة، يرجى تحديد سعر الكيلو واط أولاً في النظام.");
            }

            decimal currentPricePerKwh = systemInfo.ElectricityPricePerKwh;

            // تحويل الـ Dto إلى Entity
            var meterReading = _mapper.Map<Domain.Entities.MeterReading>(request.Dto);
            meterReading.DepartmentName = department.Name;

            // جلب القراءات السابقة
            var allDepartmentReadings = await _meterReadingService.GetByDepartmentIdAsync(request.Dto.DepartmentId);
            var lastReading = allDepartmentReadings.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

            meterReading.PreviousReading = lastReading?.CurrentReading ?? 0;

            // حساب الاستهلاك
            meterReading.CalculateConsumption(department.MaxCounter);

            // حساب التكلفة (هذه الميثود الآن تحفظ السعر داخل meterReading.PricePerUnit)
            meterReading.CalculateTotalCost(currentPricePerKwh, department.Discount);

            // حفظ السجل
            await _meterReadingService.AddAsync(meterReading);

            return _mapper.Map<MeterReadingResponseDto>(meterReading);
        }
    }
}