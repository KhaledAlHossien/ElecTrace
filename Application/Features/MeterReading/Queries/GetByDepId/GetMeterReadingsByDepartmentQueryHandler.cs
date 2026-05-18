using Application.Features.MeterReading.Queries.GetByDepId;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MeterReading.Queries.GetByDepartment
{
    public class GetMeterReadingsByDepartmentQueryHandler : IRequestHandler<GetMeterReadingsByDepartmentQuery, IEnumerable<MeterReadingResponseDto>>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public GetMeterReadingsByDepartmentQueryHandler(
            IMeterReadingService meterReadingService,
            IDepartmentService departmentService,
            IMapper mapper)
        {
            _meterReadingService = meterReadingService;
            _departmentService = departmentService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MeterReadingResponseDto>> Handle(
            GetMeterReadingsByDepartmentQuery request,
            CancellationToken cancellationToken)
        {
            var department = await _departmentService.GetByIdAsync(request.DepartmentId);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {request.DepartmentId} was not found.");
            }

            var readings = await _meterReadingService.GetByDepartmentIdAsync(request.DepartmentId);

            var result = _mapper.Map<IEnumerable<MeterReadingResponseDto>>(readings);
            return result;
        }
    }
}