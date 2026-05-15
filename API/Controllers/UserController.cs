using Application.Features.User.Command.Create;
using Application.Features.User.Command.Delete;
using Application.Features.User.Command.Update;
using Application.Features.User.Queries.GetAll;
using Application.Features.User.Queries.GetById;
using Application.Features.User.Queries.Login;
using Application.Features.User.Queries.Search;
using Application.Helper;
using Application_Contract.DTOs;
using Application_Contract.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserRequestDto createUserDto)
        {
            var result = await _mediator.Send(new CreateUserCommand(createUserDto));
            return Ok(result);
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequestDto request)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, request));

            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            return Ok(new
            {
                message = "User deleted successfully"
            });
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(result);
        }

        [HttpGet("Search/{name}")]
        public async Task<IActionResult> Search(string name)
        {
            var result = await _mediator.Send(new GetUserByNameQuery(name));
            return Ok(result);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return Ok(result);
        }


    }
}