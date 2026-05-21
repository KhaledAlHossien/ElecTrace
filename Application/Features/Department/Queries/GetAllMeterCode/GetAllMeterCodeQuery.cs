using Application_Contract.DTOs.Department;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetAllMeterCode
{
    public record class GetAllMeterCodeQuery() : IRequest<List<MeterCodeNameDto>>;
    
}
