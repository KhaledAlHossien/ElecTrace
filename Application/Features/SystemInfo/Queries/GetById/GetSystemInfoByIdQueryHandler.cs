using Application_Contract.DTOs.SystemInfo;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Application_Contract.Interfaces;
namespace Application.Features.SystemInfo.Queries.GetById
{
    public class GetSystemInfoByIdQueryHandler : IRequestHandler<GetSystemInfoByIdQuery, SystemInfoResponseDto>
    {
        private readonly ISystemInfoService _sysService;
        private readonly IMapper _mapper;

        public GetSystemInfoByIdQueryHandler(ISystemInfoService sysService, IMapper mapper)
        {
            _sysService = sysService;
            _mapper = mapper;
        }

        public async Task<SystemInfoResponseDto> Handle(GetSystemInfoByIdQuery request, CancellationToken cancellationToken)
        {
            var sysInfo = await _sysService.GetByIdAsync(request.Id);
            if (sysInfo == null) throw new KeyNotFoundException($"SystemInfo with ID {request.Id} not found.");

            return _mapper.Map<SystemInfoResponseDto>(sysInfo);
        }
    }
}
