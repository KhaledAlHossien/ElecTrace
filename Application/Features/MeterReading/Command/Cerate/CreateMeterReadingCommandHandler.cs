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
            // 1. جلب بيانات القسم للتأكد من وجوده ولأخذ معلومات العداد والاسم ومبلغ الخصم منه
            var department = await _departmentService.GetByIdAsync(request.Dto.DepartmentId);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {request.Dto.DepartmentId} not found.");
            }

            // 2. جلب السعر الحالي للكيلو واط من جدول إعدادات النظام SystemInfo
            var systemInfos = await _systemInfoService.GetAllAsync(); // أو الميثود المقابلة لديك بالجلب
            var systemInfo = systemInfos.FirstOrDefault();

            if (systemInfo == null)
            {
                throw new Exception("إعدادات النظام غير معرّفة، يرجى تحديد سعر الكيلو واط أولاً في النظام.");
            }

            decimal currentPricePerKwh = systemInfo.ElectricityPricePerKwh;

            // 3. تحويل الـ Dto القادم من الطلب إلى كائن الكيان (Entity)
            var meterReading = _mapper.Map<Domain.Entities.MeterReading>(request.Dto);

            // تخزين اسم القسم الحالي داخل حقل الـ DepartmentName بجدول القراءات كـ Snapshot
            meterReading.DepartmentName = department.Name;

            // 4. جلب القراءات السابقة لتحديد الـ PreviousReading تلقائياً
            var allDepartmentReadings = await _meterReadingService.GetByDepartmentIdAsync(request.Dto.DepartmentId);

            var lastReading = allDepartmentReadings
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefault();

            if (lastReading != null)
            {
                meterReading.PreviousReading = lastReading.CurrentReading;
            }
            else
            {
                meterReading.PreviousReading = 0;
            }

            // 5. استدعاء ميثود حساب الاستهلاك الذكية ومعالجة الالتفاف
            meterReading.CalculateConsumption(department.MaxCounter);

            // ⚠️ 6. استدعاء ميثود حساب السعر الإجمالي الكلي تلقائياً (الاستهلاك الفعلي * سعر الكهرباء) - خصم القسم المالي
            meterReading.CalculateTotalCost(currentPricePerKwh, department.Discount);

            // 7. حفظ السجل بداخل قاعدة البيانات شاملاً الاسم الثابت والسعر الكلي المحسوب بدقة
            await _meterReadingService.AddAsync(meterReading);

            // 8. تحويل النتيجة المكتملة إلى الـ Response المتوجه للفرونت إند
            return _mapper.Map<MeterReadingResponseDto>(meterReading);
        }
    }
}