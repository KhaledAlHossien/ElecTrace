using Application_Contract.DTOs.SystemInfo;
using AutoMapper;
using Domain.Entities;
using System.Collections.Generic;
namespace Application.Helper.Profiles
{
    public class SystemInfoProfile : Profile
    {
        public SystemInfoProfile()
        {
            CreateMap<SystemInfo, SystemInfoResponseDto>();
            CreateMap<UpdateSystemInfoRequestDto, SystemInfo>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
