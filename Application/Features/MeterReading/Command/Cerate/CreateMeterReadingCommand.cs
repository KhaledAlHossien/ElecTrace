using Application_Contract.DTOs.MeterReading;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.Cerate
{
    public record CreateMeterReadingCommand(CreateMeterReadingRequestDto Dto) : IRequest<MeterReadingResponseDto>;
}
