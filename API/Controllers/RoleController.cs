using Application.Features.Role.Command.Create;
using Application.Features.Role.Command.Delete;
using Application.Features.Role.Command.Update;
using Application.Features.Role.Query.GetAll;
using Application.Features.Role.Query.GetById;
using Application_Contract.DTOs.Role;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/Role")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<RoleResponseDto>> Create([FromForm] RoleRequestDto dto)
        {
            var result = await _mediator.Send(new CreateRoleCommand(dto));
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult<RoleResponseDto>> Update(int id, [FromForm] RoleRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(id, dto));
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteRoleCommand(id));
            return Ok(new { message = "Role has been deleted successfully." });
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<RoleResponseDto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<RoleResponseDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllRolesQuery());
            return Ok(result);
        }
    }
}
