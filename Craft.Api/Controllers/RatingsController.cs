using Craft.Application.Logics;
using Craft.Application.Logics.Ratings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IMediator mediator;
	public RatingsController(IMediator mediator)
	{
		this.mediator = mediator;
	}
    [HttpPost]
    public async Task<IActionResult> CreateRating([FromForm] CreateRatingCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{businessId}")]
    public async Task<IActionResult> GeBusinesstRatings(long businessId)
    {
        var result = await mediator.Send(new GetBusinessRatingsQuery { BusinessId = businessId });
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductRatings(long productId)
    {
        var result = await mediator.Send(new GetProductRatingsQuery { ProductId = productId });
        return Ok(result);
    }
}
