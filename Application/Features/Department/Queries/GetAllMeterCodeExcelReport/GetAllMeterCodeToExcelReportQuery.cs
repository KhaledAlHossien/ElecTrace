using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetAllMeterCodeExcelReport
{
    public record GetAllMeterCodeToExcelReportQuery() : IRequest<byte[]>;

}
