using Application_Contract.DTOs.SystemInfo;
using MediatR;

namespace Application.Features.SystemInfo.Queries.GetAmeenConnection
{
    public record GetAmeenConnectionQuery() : IRequest<AmeenConnectionDto>;
}
