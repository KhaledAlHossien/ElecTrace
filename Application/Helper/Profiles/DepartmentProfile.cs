using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.User;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helper.Profiles
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentRequestDto, Department>();

            CreateMap<Department, DepartmentResponseDto>();

            CreateMap<UpdateDepartmentRequestDto, Department>()
                  .ForMember(dest => dest.ConversionFactor, opt => opt.Condition(src => src.ConversionFactor.HasValue))
                  .ForMember(dest => dest.MaxCounter, opt => opt.Condition(src => src.MaxCounter.HasValue))
                  .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
