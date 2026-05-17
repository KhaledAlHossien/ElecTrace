using Application_Contract.DTOs.MeterReading;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helper.Profiles
{
    public class MeterReadingProfile : Profile
    {
        public MeterReadingProfile()
        {
            CreateMap<MeterReading, MeterReadingResponseDto>();

            CreateMap<CreateMeterReadingRequestDto, MeterReading>();

            CreateMap<UpdateMeterReadingRequestDto, MeterReading>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
