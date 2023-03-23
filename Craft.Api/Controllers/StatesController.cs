using Craft.Application.Logics.States.Command;
using Craft.Application.Logics.States.Quries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatesController : ControllerBase
{
    private readonly IMediator mediator;
	public StatesController(IMediator mediator)
	{
		this.mediator = mediator;
	}

    [HttpPost]
    public async Task<IActionResult> CreateState([FromForm] CreateStateCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateState([FromRoute] long Id, [FromForm] UpdateStateCommand command)
    {
        if (command != null)
        {
            command.StateId = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await mediator
            .Send(new GetStatesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await mediator.Send(new GetStatesByIdQuery(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long Id)
    {
        await mediator.Send(new DeleteStateCommand { Id = Id });

        return Ok("Removed Successfully");
    }
}
