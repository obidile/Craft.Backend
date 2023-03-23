using Craft.Application.Logics.Schools.Command;
using Craft.Application.Logics.Schools.Quries;
using Craft.Application.Logics.States.Command;
using Craft.Application.Logics.States.Quries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchoolsController : ControllerBase
{
    private readonly IMediator mediator;
    public SchoolsController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> CreateSchool([FromForm] CreateSchoolCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSSchool([FromRoute] long Id, [FromForm] UpdateSchoolCommand command)
    {
        if (command != null)
        {
            command.SchoolId = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string Name)
    {
        return Ok(await mediator
            .Send(new GetSchoolsQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await mediator.Send(new GetSchoolByIdQuery(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] long Id)
    {
        await mediator.Send(new DeleteSchoolCommand { Id = Id });

        return Ok("Removed Successfully");
    }
}
