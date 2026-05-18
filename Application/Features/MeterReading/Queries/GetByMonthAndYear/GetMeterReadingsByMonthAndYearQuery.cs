using Application_Contract.DTOs.MeterReading;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Queries.GetByMonthAndYear
{
    public record GetMeterReadingsByMonthAndYearQuery(Months Month, int Year)
        : IRequest<IEnumerable<MeterReadingResponseDto>>;
}
