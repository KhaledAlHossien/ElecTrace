using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.Delete
{
    public record DeleteMeterReadingCommand(int Id) : IRequest<bool>;
}
