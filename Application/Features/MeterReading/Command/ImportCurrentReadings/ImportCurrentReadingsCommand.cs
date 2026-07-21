using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.ImportCurrentReadings
{
  public record ImportCurrentReadingsCommand(Stream FileStream, Months Month, int Year) : IRequest<bool>;

}
