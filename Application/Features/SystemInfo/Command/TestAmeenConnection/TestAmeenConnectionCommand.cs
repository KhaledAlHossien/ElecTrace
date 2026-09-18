using Application_Contract.DTOs.SystemInfo;
using MediatR;

namespace Application.Features.SystemInfo.Command.TestAmeenConnection
{
    public record TestAmeenConnectionCommand(UpdateAmeenConnectionRequestDto Dto) : IRequest<AmeenConnectionTestResultDto>;
}
