using Application.Features.MeterReading.Command.ImportReadings;
using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.ImportCurrentReadings
{
  internal class ImportCurrentReadingsCommandHandler : IRequestHandler<ImportCurrentReadingsCommand, bool>
  {
    private readonly IMeterReadingService _service;
    public ImportCurrentReadingsCommandHandler(IMeterReadingService service) => _service = service;

    public async Task<bool> Handle(ImportCurrentReadingsCommand request, CancellationToken ct)
    {
      return await _service.ImportCurrentReadingsFromExcel(request.FileStream, request.Month, request.Year);
    }
  }
}
