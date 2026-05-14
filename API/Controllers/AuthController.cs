using Application.Features.User.Command.Logout;
using Application.Features.User.Queries.Login; 
using Application_Contract.DTOs;
using Application_Contract.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]
        [AllowAnonymous] 
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _mediator.Send(new LoginQuery(loginDto));
            return Ok(result);
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            var result = await _mediator.Send(new LogoutCommand(authHeader));

            if (result)
            {
                return Ok(new { Message = "Logged out successfully" });
            }

            return BadRequest(new { Message = "Logout failed" });
        }
    }
}