using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using MediatR;

namespace Application.Features.SystemInfo.Command.TestAmeenConnection
{
    public class TestAmeenConnectionCommandHandler : IRequestHandler<TestAmeenConnectionCommand, AmeenConnectionTestResultDto>
    {
        private readonly IAmeenConnectionService _ameenConnection;

        public TestAmeenConnectionCommandHandler(IAmeenConnectionService ameenConnection)
        {
            _ameenConnection = ameenConnection;
        }

        public async Task<AmeenConnectionTestResultDto> Handle(TestAmeenConnectionCommand request, CancellationToken cancellationToken)
        {
            return await _ameenConnection.TestAsync(request.Dto);
        }
    }
}
