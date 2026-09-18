using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using MediatR;

namespace Application.Features.SystemInfo.Queries.GetAmeenConnection
{
    public class GetAmeenConnectionQueryHandler : IRequestHandler<GetAmeenConnectionQuery, AmeenConnectionDto>
    {
        private readonly IAmeenConnectionService _ameenConnection;

        public GetAmeenConnectionQueryHandler(IAmeenConnectionService ameenConnection)
        {
            _ameenConnection = ameenConnection;
        }

        public async Task<AmeenConnectionDto> Handle(GetAmeenConnectionQuery request, CancellationToken cancellationToken)
        {
            return await _ameenConnection.GetAsync();
        }
    }
}
