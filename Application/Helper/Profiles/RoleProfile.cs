using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Application_Contract.DTOs.Role;

namespace Application.Helper.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>().ReverseMap()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<RoleRequestDto, Role>().ReverseMap()
                  .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
