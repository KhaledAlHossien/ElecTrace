using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Queries.GetByRange
{
    public class GetMeterReadingsByDateRangeQueryHandler:IRequestHandler<GetMeterReadingsByDateRangeQuery, IEnumerable<MeterReadingResponseDto>>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IMapper _mapper;

        public GetMeterReadingsByDateRangeQueryHandler(IMeterReadingService meterReadingService, IMapper mapper)
        {
            _meterReadingService = meterReadingService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MeterReadingResponseDto>> Handle(
            GetMeterReadingsByDateRangeQuery request,
            CancellationToken cancellationToken)
        {
            var readings = await _meterReadingService.GetByDateRangeAsync(request.StartDate, request.EndDate);

            var result = _mapper.Map<IEnumerable<MeterReadingResponseDto>>(readings);

            return result;
        }
    }
}
