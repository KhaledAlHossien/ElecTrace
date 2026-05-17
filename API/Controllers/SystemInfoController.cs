using Application.Features.SystemInfo.Command;
using Application.Features.SystemInfo.Queries.GetAll;
using Application.Features.SystemInfo.Queries.GetById;
using Application_Contract.DTOs.SystemInfo;
using MediatR;
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
    }
}
