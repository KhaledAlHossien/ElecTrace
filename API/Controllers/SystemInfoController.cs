using Application.Features.SystemInfo.Command;
using Application.Features.SystemInfo.Command.TestAmeenConnection;
using Application.Features.SystemInfo.Command.UpdateAmeenConnection;
using Application.Features.SystemInfo.Queries.GetAll;
using Application.Features.SystemInfo.Queries.GetAmeenConnection;
using Application.Features.SystemInfo.Queries.GetById;
using Application_Contract.DTOs.SystemInfo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/SystemInfo")]
    [ApiController]
    public class SystemInfoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemInfoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllSystemInfoQuery());
            return Ok(result);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetSystemInfoByIdQuery(id));
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSystemInfoRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateSystemInfoCommand(id, dto));
            return Ok(result);
        }

        // ===== اتصال نظام الأمين =====
        // محمية بتسجيل الدخول: بتتعامل مع بيانات اعتماد قاعدة بيانات

        [Authorize]
        [HttpGet("AmeenConnection")]
        public async Task<IActionResult> GetAmeenConnection()
        {
            var result = await _mediator.Send(new GetAmeenConnectionQuery());
            return Ok(result);
        }

        [Authorize]
        [HttpPut("AmeenConnection")]
        public async Task<IActionResult> UpdateAmeenConnection([FromBody] UpdateAmeenConnectionRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateAmeenConnectionCommand(dto));
            return Ok(result);
        }

        [Authorize]
        [HttpPost("AmeenConnection/Test")]
        public async Task<IActionResult> TestAmeenConnection([FromBody] UpdateAmeenConnectionRequestDto dto)
        {
            var result = await _mediator.Send(new TestAmeenConnectionCommand(dto));
            return Ok(result);
        }
    }
}
