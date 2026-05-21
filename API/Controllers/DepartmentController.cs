using Application.Features.Department.Command.Create;
using Application.Features.Department.Command.Delete;
using Application.Features.Department.Command.Update;
using Application.Features.Department.Queries.GetAll;
using Application.Features.Department.Queries.GetAllMeterCode;
using Application.Features.Department.Queries.GetAllMeterCodeExcelReport;
using Application.Features.Department.Queries.GetById;
using Application.Features.Department.Queries.Search;

using Application_Contract.DTOs.Department;
using Application_Contract.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("api/Department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentRequestDto dto)
        {
            var result = await _mediator.Send(new CreateDepartmentCommand(dto));

            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateDepartmentCommand(id, dto));

            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteDepartmentCommand(id));
            return Ok(new { Message = "Department deleted successfully" });
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetDepartmentByIdQuery(id));

            return Ok(result);
        }
        [HttpGet("Search/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _mediator.Send(new GetDepartmentByNameQuery(name));
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDepartmentsQuery());
            return Ok(result);
        }

        [HttpGet("GetAllMeterCode")]
        public async Task<IActionResult> GetAllMeterCode()
        {
            var result = await _mediator.Send(new GetAllMeterCodeQuery());

            return Ok(result);
        }

        [HttpGet("GetAllMeterCodeToExcel")]
        public async Task<IActionResult> ExportMeterCodesToExcel()
        {
            var fileBytes = await _mediator.Send(new GetAllMeterCodeToExcelReportQuery());

            if (fileBytes == null || fileBytes.Length == 0)
                return NotFound("لا توجد بيانات لتصديرها.");

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MeterCodes_{DateTime.Now:yyyyMMdd}.xlsx"
            );
        }

    }
}

