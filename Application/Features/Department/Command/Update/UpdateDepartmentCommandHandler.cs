using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Department.Command.Update
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentResponseDto>
    {
        private readonly IDepartmentService _deptService;
        private readonly IMapper _mapper;

        public UpdateDepartmentCommandHandler(IDepartmentService deptService, IMapper mapper)
        {
            _deptService = deptService;
            _mapper = mapper;
        }

        public async Task<DepartmentResponseDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dept = await _deptService.GetByIdAsync(request.Id);
            if (dept == null) throw new KeyNotFoundException("Department not found.");

            var dto = request.DeptDto;

            if (!string.IsNullOrEmpty(dto.Name)) dept.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.MeterCode) && dto.MeterCode != dept.MeterCode)
            {
                var existing = await _deptService.GetByMeterCodeAsync(dto.MeterCode);
                if (existing != null) throw new Exception("Meter Code already in use.");
                dept.MeterCode = dto.MeterCode;
            }

            if (dto.ConversionFactor.HasValue)
                dept.ConversionFactor = dto.ConversionFactor.Value;

            if (dto.MaxCounter.HasValue)
                dept.MaxCounter = dto.MaxCounter.Value;

            // ⚠️ تم الإصلاح هنا: فحص القيمة وتمريرها باستخدام .Value لإنهاء خطأ الـ Cast
            if (dto.Discount.HasValue)
                dept.Discount = dto.Discount.Value;

            await _deptService.UpdateAsync(dept);

            return _mapper.Map<DepartmentResponseDto>(dept);
        }
    }
}