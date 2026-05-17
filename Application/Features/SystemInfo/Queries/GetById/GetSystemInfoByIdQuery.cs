using Application_Contract.DTOs.SystemInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SystemInfo.Queries.GetById
{
    public record GetSystemInfoByIdQuery(int Id) : IRequest<SystemInfoResponseDto>;
}
