using Application_Contract.DTOs.SystemInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SystemInfo.Queries.GetAll
{
    public record GetAllSystemInfoQuery() : IRequest<IEnumerable<SystemInfoResponseDto>>;
}
