using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MeterReading.Command.ImportReadings
{
    public class ImportReadingsCommandHandler : IRequestHandler<ImportReadingsCommand, bool>
    {
        private readonly IMeterReadingService _service;
        public ImportReadingsCommandHandler(IMeterReadingService service) => _service = service;

        public async Task<bool> Handle(ImportReadingsCommand request, CancellationToken ct)
        {
            return await _service.ImportReadingsFromExcel(request.FileStream, request.Month, request.Year);
        }
    }
}
