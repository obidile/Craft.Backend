using Craft.Application.Logics.Businesses.Command;
using Craft.Application.Logics.Businesses.Quries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BusinessesController : ControllerBase
{
    private readonly IMediator mediator;
	public BusinessesController(IMediator mediator)
	{
		this.mediator = mediator;
	}

    [HttpPost]
    public async Task<IActionResult> CreateBusiness([FromForm] CreateBusinessCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness([FromRoute] long Id, [FromForm] UpdateBusinessCommand command)
    {
        if (command != null)
        {
            command.Id = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string BusinessMail, string BusinessName)
    {
        return Ok(await mediator
            .Send(new GetBusinessesQuery() { BusinessMail = BusinessMail, BusinessName = BusinessName }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await mediator.Send(new GetBusinessByIdQuery(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long Id)
    {
        await mediator.Send(new DeleteBusinessCommand { Id = Id });

        return Ok("Removed Successfully");
    }

}
