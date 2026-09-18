using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using MediatR;

namespace Application.Features.SystemInfo.Command.UpdateAmeenConnection
{
    public class UpdateAmeenConnectionCommandHandler : IRequestHandler<UpdateAmeenConnectionCommand, AmeenConnectionDto>
    {
        private readonly IAmeenConnectionService _ameenConnection;

        public UpdateAmeenConnectionCommandHandler(IAmeenConnectionService ameenConnection)
        {
            _ameenConnection = ameenConnection;
        }

        public async Task<AmeenConnectionDto> Handle(UpdateAmeenConnectionCommand request, CancellationToken cancellationToken)
        {
            return await _ameenConnection.UpdateAsync(request.Dto);
        }
    }
}
