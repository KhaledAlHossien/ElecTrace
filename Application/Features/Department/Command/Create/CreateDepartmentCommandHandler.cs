using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System.Threading; 
using System.Threading.Tasks; 
using DepartmentEntity = Domain.Entities.Department;

namespace Application.Features.Department.Command.Create
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentResponseDto>
    {
        private readonly IDepartmentService _deptService;
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IDepartmentService deptService, IMapper mapper)
        {
            _deptService = deptService;
            _mapper = mapper;
        }

        public async Task<DepartmentResponseDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var existingDept = await _deptService.GetByMeterCodeAsync(request.DeptDto.MeterCode);
            if (existingDept != null)
            {
                throw new Exception($"Meter Code '{request.DeptDto.MeterCode}' is already in use.");
            }

            var dept = _mapper.Map<DepartmentEntity>(request.DeptDto);

            await _deptService.CreateAsync(dept);

            return _mapper.Map<DepartmentResponseDto>(dept);
        }
    }
}