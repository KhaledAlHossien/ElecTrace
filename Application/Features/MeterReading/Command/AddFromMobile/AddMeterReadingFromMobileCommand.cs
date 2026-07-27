using Application_Contract.DTOs.MeterReading;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.AddFromMobile
{
  public record AddMeterReadingFromMobileCommand(CreateMeterReadingRequestDto Dto, bool UnusualAccept, Lang Lang) : IRequest<MeterReadingResponseDto>;

}
