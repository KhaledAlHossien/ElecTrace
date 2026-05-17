using Application_Contract.DTOs.SystemInfo;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.SystemInfo.Command
{
    public class UpdateSystemInfoCommandHandler : IRequestHandler<UpdateSystemInfoCommand, SystemInfoResponseDto>
    {
        private readonly ISystemInfoService _sysService;
        private readonly IMapper _mapper;

        public UpdateSystemInfoCommandHandler(ISystemInfoService sysService, IMapper mapper)
        {
            _sysService = sysService;
            _mapper = mapper;
        }

        public async Task<SystemInfoResponseDto> Handle(UpdateSystemInfoCommand request, CancellationToken cancellationToken)
        {
            var sysInfo = await _sysService.GetByIdAsync(request.Id);
            if (sysInfo == null)
            {
                throw new KeyNotFoundException($"SystemInfo with ID {request.Id} not found.");
            }

            _mapper.Map(request.Dto, sysInfo);

            await _sysService.UpdateAsync(sysInfo);
            return _mapper.Map<SystemInfoResponseDto>(sysInfo);
        }
    }
}
