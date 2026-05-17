using Application_Contract.DTOs.SystemInfo;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SystemInfo.Command
{
    public record UpdateSystemInfoCommand(int Id, UpdateSystemInfoRequestDto Dto) : IRequest<SystemInfoResponseDto>;
}
