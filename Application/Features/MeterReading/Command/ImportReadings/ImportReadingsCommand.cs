using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.ImportReadings
{
    public record ImportReadingsCommand(Stream FileStream, Months Month, int Year) : IRequest<bool>;
}
