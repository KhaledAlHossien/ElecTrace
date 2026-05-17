using Application_Contract.DTOs.MeterReading;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Queries.GetByRange
{
    public record GetMeterReadingsByDateRangeQuery(DateTime StartDate, DateTime EndDate)
         : IRequest<IEnumerable<MeterReadingResponseDto>>;
}
