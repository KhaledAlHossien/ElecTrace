using Application.Features.MeterReading.Command.Cerate;
using Application.Features.MeterReading.Command.Delete;
using Application.Features.MeterReading.Queries.GetByDepId;
using Application.Features.MeterReading.Queries.GetByRange;
using Application_Contract.DTOs.MeterReading;
using Application_Contract.Interfaces;
using AutoMapper;
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

        [HttpGet("GetByDateRange")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _mediator.Send(new GetMeterReadingsByDateRangeQuery(startDate, endDate));
            return Ok(result);
        }
        [HttpGet("GetByDepartment/{departmentId}")]
        public async Task<IActionResult> GetByDepartmentId(int departmentId)
        {
            var result = await _mediator.Send(new GetMeterReadingsByDepartmentQuery(departmentId));
            return Ok(result); 
        }
    }
}
