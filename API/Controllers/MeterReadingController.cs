using Application.Features.MeterReading.Command.Cerate;
using Application.Features.MeterReading.Command.Delete;
using Application.Features.MeterReading.Command.ImportReadings;
using Application.Features.MeterReading.Queries.GetByDepId;
using Application.Features.MeterReading.Queries.GetByMonthAndYear;
using Application.Features.Report.Queries.GetElectricityExcelReport;
using Application.Features.Report.Queries.Invoice;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/MeterReading")]
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMeterReadingService _meterReadingService;
        private readonly IMapper _mapper;

        public MeterReadingController(IMediator mediator, IMeterReadingService meterReadingService, IMapper mapper)
        {
            _mediator = mediator;
            _meterReadingService = meterReadingService;
            _mapper = mapper;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateMeterReadingRequestDto dto)
        {
            var result = await _mediator.Send(new CreateMeterReadingCommand(dto));
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteMeterReadingCommand(id));
            return Ok(new { Message = "Meter reading deleted successfully" });
        }

        [HttpGet("GetByMonthAndYear")]
        public async Task<IActionResult> GetByMonthAndYear([FromQuery] Domain.Enums.Months month, [FromQuery] int year)
        {
            var result = await _mediator.Send(new GetMeterReadingsByMonthAndYearQuery(month, year));
            return Ok(result);
        }
        [HttpGet("GetByDepartment/{departmentId}")]
        public async Task<IActionResult> GetByDepartmentId(int departmentId)
        {
            var result = await _mediator.Send(new GetMeterReadingsByDepartmentQuery(departmentId));
            return Ok(result); 
        }
        [HttpGet("DownloadElectricityExcelReport")]
        public async Task<IActionResult> DownloadElectricityExcelReport([FromQuery] Months month, [FromQuery] int year)
        {
            var fileBytes = await _mediator.Send(new GetElectricityExcelReportQuery(month, year));
            if (fileBytes.Length == 0)
            {
                return NotFound(new { message = $"لا توجد قراءات مسجلة للشهر {(int)month} لعام {year}." });
            }

            string fileName = $"Electricity_Report_{year}_{((int)month)}.xlsx";

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, fileName);
        }
        [HttpGet("DownloadAllInvoices")]
        public async Task<IActionResult> DownloadAllInvoices([FromQuery] Domain.Enums.Months month, [FromQuery] int year)
        {
            var fileBytes = await _mediator.Send(new GetAllInvoicesExcelQuery(month, year));
            if (fileBytes.Length == 0)
            {
                return NotFound(new { message = $"لا توجد فواتير أو قراءات مسجلة للشهر {(int)month} لعام {year}." });
            }

            string fileName = $"All_Invoices_{year}_{((int)month)}.xlsx";

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(fileBytes, contentType, fileName);
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file, [FromQuery] Months month, [FromQuery] int year)
        {
            if (file == null) return BadRequest("الملف مفقود");

            using var stream = file.OpenReadStream();
            var command = new ImportReadingsCommand(stream, month, year);

            var result = await _mediator.Send(command);
            return result ? Ok("تم الاستيراد بنجاح") : BadRequest("فشل الاستيراد");
        }
    }
}
