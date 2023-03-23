using Craft.Application.Logics.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Admin")]
    public async Task<ActionResult<string>> GetAdminDashboard()
    {
        var query = new GetAdminDashboardQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("vendor")]
    public async Task<IActionResult> GetVendorDashboard()
    {
        var query = new GetVendorDashboardQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
