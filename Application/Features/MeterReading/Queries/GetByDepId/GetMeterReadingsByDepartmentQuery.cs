using Application_Contract.DTOs.MeterReading;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Queries.GetByDepId
{
    public record GetMeterReadingsByDepartmentQuery(int DepartmentId)
        : IRequest<IEnumerable<MeterReadingResponseDto>>;
}
