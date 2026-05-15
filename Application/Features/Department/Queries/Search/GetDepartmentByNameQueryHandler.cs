using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.Search
{
    public class GetDepartmentByNameQueryHandler : IRequestHandler<GetDepartmentByNameQuery, IEnumerable<DepartmentResponseDto>>
    {
        private readonly IDepartmentService _deptService;
        private readonly IMapper _mapper;

        public GetDepartmentByNameQueryHandler(IDepartmentService deptService, IMapper mapper)
        {
            _deptService = deptService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentResponseDto>> Handle(GetDepartmentByNameQuery request, CancellationToken cancellationToken)
        {
            var departments = await _deptService.GetByNameAsync(request.Name);

            return _mapper.Map<IEnumerable<DepartmentResponseDto>>(departments);
        }
    }
}
