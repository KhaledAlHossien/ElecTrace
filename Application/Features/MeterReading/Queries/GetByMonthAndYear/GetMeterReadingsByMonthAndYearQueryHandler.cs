using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.MeterReading.Queries.GetByMonthAndYear
{
    public class GetMeterReadingsByMonthAndYearQueryHandler
        : IRequestHandler<GetMeterReadingsByMonthAndYearQuery, IEnumerable<MeterReadingResponseDto>>
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService; // استخدام الواجهة الخاصة بك بدلاً من UserManager

        public GetMeterReadingsByMonthAndYearQueryHandler(
            IMeterReadingService meterReadingService,
            IMapper mapper,
            IUserService userService) // حقن الواجهة
        {
            _meterReadingService = meterReadingService;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IEnumerable<MeterReadingResponseDto>> Handle(
            GetMeterReadingsByMonthAndYearQuery request, CancellationToken cancellationToken)
        {
            var readings = await _meterReadingService.GetByMonthAndYearAsync(request.Month, request.Year);
            var dtos = _mapper.Map<List<MeterReadingResponseDto>>(readings ?? new List<Domain.Entities.MeterReading>());

            // حلقة لجلب الاسم لكل قراءة باستخدام الخدمة الخاصة بك
            foreach (var dto in dtos)
            {
                if (!string.IsNullOrEmpty(dto.CreatedByUserId))
                {
                    // استدعاء الدالة التي أضفناها في IUserService
                    dto.CreatedByUserName = await _userService.GetUserNameByIdAsync(dto.CreatedByUserId) ?? "غير معروف";
                }
            }

            return dtos;
        }
    }
}