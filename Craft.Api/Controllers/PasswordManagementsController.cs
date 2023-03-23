using Craft.Application.Logics.PasswordManagement;
using Craft.Application.Logics.Users.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PasswordManagementsController : ControllerBase
{
    private readonly IMediator mediator;
    public PasswordManagementsController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPut("{ChangePassword}")]
    public async Task<IActionResult> UpdateUser([FromRoute] long Id, [FromForm] ChangePasswordCommand command)
    {
        if (command != null)
        {
            command.UserId = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
