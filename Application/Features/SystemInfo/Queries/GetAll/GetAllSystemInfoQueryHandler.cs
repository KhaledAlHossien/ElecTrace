using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SystemInfo.Queries.GetAll
{
    public class GetAllSystemInfoQueryHandler : IRequestHandler<GetAllSystemInfoQuery, IEnumerable<SystemInfoResponseDto>>
    {
        private readonly ISystemInfoService _sysService;
        private readonly IMapper _mapper;

        public GetAllSystemInfoQueryHandler(ISystemInfoService sysService, IMapper mapper)
        {
            _sysService = sysService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SystemInfoResponseDto>> Handle(GetAllSystemInfoQuery request, CancellationToken cancellationToken)
        {
            var list = await _sysService.GetAllAsync();
            return _mapper.Map<IEnumerable<SystemInfoResponseDto>>(list);
        }
    }
}
