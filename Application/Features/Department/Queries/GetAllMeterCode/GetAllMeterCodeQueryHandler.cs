using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetAllMeterCode
{
    public class GetAllMeterCodeQueryHandler : IRequestHandler<GetAllMeterCodeQuery, List<MeterCodeNameDto>>
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        public GetAllMeterCodeQueryHandler(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        public async Task<List<MeterCodeNameDto>> Handle(GetAllMeterCodeQuery request, CancellationToken cancellationToken)
        {
            var departments = await _departmentService.GetAllAsync();

            return _mapper.Map<List<MeterCodeNameDto>>(departments);
        }

       
    }
}
