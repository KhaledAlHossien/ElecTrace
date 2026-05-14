using Application.Features.User.Command.Create;
using Application.Features.User.Queries.Login; 
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
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var result = await _mediator.Send(new CreateUserCommand(createUserDto));
            return Ok(result);
        }

        
    }
}