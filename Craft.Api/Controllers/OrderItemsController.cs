using Craft.Application.Logics.OrderItems.Quries;
using Craft.Application.Logics.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemsController : ControllerBase
{
    private readonly IMediator _mediator;
	public OrderItemsController(IMediator mediator)
	{
		_mediator = mediator;
	}

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(long id)
    {
        var query = new GetOrderItemByIdQuery(id);
        var orderItems = await _mediator.Send(query);
        return Ok(orderItems);
    }

    [HttpGet("orders/{orderId}/items")]
    public async Task<ActionResult> GetOrderItemsByOrderId(long orderId)
    {
        var query = new GetOrderItemsByOrderIdQuery(orderId);
        var orderItems = await _mediator.Send(query);
        return Ok(orderItems);
    }
}
