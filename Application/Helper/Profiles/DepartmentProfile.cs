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

            CreateMap<Department, DepartmentResponseDto>().ReverseMap();

            CreateMap<UpdateDepartmentRequestDto, Department>()
      .ForMember(dest => dest.ConversionFactor, opt => opt.Condition(src => src.ConversionFactor.HasValue))
      // ⚠️ أضف هذا السطر الصريح لحقل الخصم لمنع التضارب
      .ForMember(dest => dest.Discount, opt => opt.Condition(src => src.Discount >= 0))
      // تأكد أن الاسم يطابق حقل الـ Entity (إذا كان DiscountAmount غيره لـ DiscountAmount)

      // إذا كان عندك حقول نصية متل Name أو MeterCode بدك تحميها من الـ Null، اذكرها صراحة كالتالي:
      .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
      .ForMember(dest => dest.MeterCode, opt => opt.Condition(src => src.MeterCode != null));
            CreateMap<Department, MeterCodeNameDto>();
        }
    }
}
