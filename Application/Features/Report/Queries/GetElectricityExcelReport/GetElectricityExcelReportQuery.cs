using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Report.Queries.GetElectricityExcelReport
{
    public record GetElectricityExcelReportQuery(Months Month, int Year) : IRequest<byte[]>;
}
