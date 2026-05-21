using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.Features.Department.Queries.GetAllMeterCodeExcelReport
{
       public class GetAllMeterCodeToExcelReportQueryHandler : IRequestHandler<GetAllMeterCodeToExcelReportQuery, byte[]>
        {
            private readonly IDepartmentService _departmentService;
            private readonly IMapper _mapper;
            private readonly IElectricityReportService _excelService;

            public GetAllMeterCodeToExcelReportQueryHandler(
                IDepartmentService departmentService,
                IMapper mapper,
                IElectricityReportService excelService)
            {
                _departmentService = departmentService;
                _mapper = mapper;
                _excelService = excelService;
            }

            public async Task<byte[]> Handle(GetAllMeterCodeToExcelReportQuery request, CancellationToken cancellationToken)
            {
                var departments = await _departmentService.GetAllAsync();
                var data = _mapper.Map<List<MeterCodeNameDto>>(departments);

                if (data == null || data.Count == 0)
                    return new byte[0];

                return _excelService.GenerateDepartmentsExcel(data, "قائمة رموز العدادات");
            }
        }
    }

