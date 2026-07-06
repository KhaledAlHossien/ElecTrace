using Application.Features.MeterReading.Command.ImportReadings;
using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Import
{
    internal class ImportDepartmentsCommandHandler : IRequestHandler<ImportDepartmentsCommand, bool>
    {
        private readonly IDepartmentService _service;
        public ImportDepartmentsCommandHandler(IDepartmentService service) => _service = service;

        public async Task<bool> Handle(ImportDepartmentsCommand request, CancellationToken ct)
        {
            return await _service.ImportFromExcel(request.FileStream);
        }
    }
}
