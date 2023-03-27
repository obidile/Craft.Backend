using Craft.Application.Logics.Countries.Command;
using Craft.Application.Logics.Countries.Quries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IMediator mediator;
	public CountriesController(IMediator mediator)
	{
		this.mediator = mediator;
	}
    [HttpPost]
    public async Task<IActionResult> CreateCountry([FromForm] CreateCountryCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness([FromRoute] long Id, [FromForm] UpdateCountryCommand command)
    {
        if (command != null)
        {
            command.CountryId = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string Name)
    {
        return Ok(await mediator
            .Send(new GetCountriesQuery() { Name = Name }));
    }

    [HttpGet("{GetByid}")]
    public async Task<IActionResult> Get([FromRoute] long id)
    {
        return Ok(await mediator.Send(new GetCountryByIdQuery(id)));
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete([FromRoute] long Id)
    {
        await mediator.Send(new DeleteCountryCommand { Id = Id });

        return Ok("Removed Successfully");
    }
}
