using Craft.Application.Logics.Users.Command;
using Craft.Application.Logics.Users.Quries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator mediator;
	public UsersController(IMediator mediator)
	{
		this.mediator = mediator;
	}

	[HttpPost]
    public async Task<IActionResult> CreateUser([FromForm] CreateUserCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] long Id, [FromForm] UpdateUserCommand command)
    {
        if (command != null)
        {
            command.Id = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string MailAddress, bool IsActive, bool Deactivated)
    {
        return Ok(await mediator
            .Send(new GetUsersQuery() { MailAddress = MailAddress, IsActive = IsActive, Deactivated = Deactivated }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await mediator.Send(new GetUserByIdQuery(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long Id)
    {
        await mediator.Send(new DeleteUserCommand { Id = Id });

        return Ok ("Removed Successfully");
    }
}
