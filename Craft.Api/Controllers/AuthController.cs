using Craft.Application.Logics.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
  
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(token);
    }
}
