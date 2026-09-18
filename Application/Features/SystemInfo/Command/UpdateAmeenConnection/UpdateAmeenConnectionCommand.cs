using Application_Contract.DTOs.SystemInfo;
using MediatR;

namespace Application.Features.SystemInfo.Command.UpdateAmeenConnection
{
    public record UpdateAmeenConnectionCommand(UpdateAmeenConnectionRequestDto Dto) : IRequest<AmeenConnectionDto>;
}
