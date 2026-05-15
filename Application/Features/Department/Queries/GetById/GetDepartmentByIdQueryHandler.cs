using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetById
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentResponseDto>
    {
        private readonly IDepartmentService _deptService;
        private readonly IMapper _mapper;

        public GetDepartmentByIdQueryHandler(IDepartmentService deptService, IMapper mapper)
        {
            _deptService = deptService;
            _mapper = mapper;
        }

        public async Task<DepartmentResponseDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var dept = await _deptService.GetByIdAsync(request.Id);

            if (dept == null)
            {
                throw new KeyNotFoundException($"Department with ID {request.Id} not found.");
            }

            return _mapper.Map<DepartmentResponseDto>(dept);
        }
    }
}
