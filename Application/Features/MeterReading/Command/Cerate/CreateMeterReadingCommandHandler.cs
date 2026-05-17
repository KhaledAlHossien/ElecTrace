using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq; // تأكد من وجودها لاستخدام الـ LINQ
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MeterReading.Command.Cerate
{
    public class CreateMeterReadingCommandHandler : IRequestHandler<CreateMeterReadingCommand, MeterReadingResponseDto>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public CreateMeterReadingCommandHandler(IMeterReadingService meterReadingService,IDepartmentService departmentService,IMapper mapper)
        {
            _meterReadingService = meterReadingService;
            _departmentService = departmentService;
            _mapper = mapper;
        }

        public async Task<MeterReadingResponseDto> Handle(CreateMeterReadingCommand request, CancellationToken cancellationToken)
        {
            var department = await _departmentService.GetByIdAsync(request.Dto.DepartmentId);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {request.Dto.DepartmentId} not found.");
            }

            var meterReading = _mapper.Map<Domain.Entities.MeterReading>(request.Dto);

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

            meterReading.CalculateConsumption(department.MaxCounter);

            await _meterReadingService.AddAsync(meterReading);

            return _mapper.Map<MeterReadingResponseDto>(meterReading);
        }
    }
}