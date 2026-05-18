using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Queries.GetByMonthAndYear
{
    namespace Application.Features.MeterReading.Queries.GetByMonthAndYear
    {
        public class GetMeterReadingsByMonthAndYearQueryHandler
            : IRequestHandler<GetMeterReadingsByMonthAndYearQuery, IEnumerable<MeterReadingResponseDto>>
        {
            private readonly IMeterReadingService _meterReadingService;
            private readonly IMapper _mapper;

            public GetMeterReadingsByMonthAndYearQueryHandler(IMeterReadingService meterReadingService, IMapper mapper)
            {
                _meterReadingService = meterReadingService;
                _mapper = mapper;
            }

            public async Task<IEnumerable<MeterReadingResponseDto>> Handle(
                GetMeterReadingsByMonthAndYearQuery request,
                CancellationToken cancellationToken)
            {
                var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);

                if (readings == null || !readings.Any())
                {
                    throw new KeyNotFoundException($"No meter readings found for Month: {request.Month} in Year: {request.Year}.");
                }

                return _mapper.Map<IEnumerable<MeterReadingResponseDto>>(readings);
            }
        }
    }
}
