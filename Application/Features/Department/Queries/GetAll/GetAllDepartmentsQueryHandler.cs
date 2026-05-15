using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetAll
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentResponseDto>>
    {
        private readonly IDepartmentService _deptService;
        private readonly IMapper _mapper;

        public GetAllDepartmentsQueryHandler(IDepartmentService deptService, IMapper mapper)
        {
            _deptService = deptService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentResponseDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var departments = await _deptService.GetAllAsync();

            return _mapper.Map<IEnumerable<DepartmentResponseDto>>(departments);
        }
    }
}
