using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Report.Queries.Invoice
{
    public record GetAllInvoicesExcelQuery(Months Month, int Year) : IRequest<byte[]>;
}
