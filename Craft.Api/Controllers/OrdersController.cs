using Craft.Application.Logics.Orders.Command;
using Craft.Application.Logics.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(long orderId)
    {
        var command = new CancelOrderCommand { OrderId = orderId };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllOrders()
    {
        var query = new GetAllOrdersQuery();
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrderById(long id)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await _mediator.Send(query);
        return Ok(order);
    }

    [HttpGet("one-hour-ago")]
    public async Task<ActionResult> GetOrdersPlacedOneHourAgo()
    {
        var query = new GetOrdersPlacedOneHourAgoQuery();
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }
}
