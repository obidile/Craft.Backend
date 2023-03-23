using Craft.Application.Common.Models;
using Craft.Application.Logics.Businesses.Command;
using Craft.Application.Logics.Products.Command;
using Craft.Application.Logics.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Craft.Api.Controllers;
[Route("api/[controller]")]
[ApiController]

public class ProductsController : ControllerBase
{
    private readonly IMediator mediator;
	public ProductsController(IMediator mediator)
	{
		this.mediator = mediator;
	}

    [HttpPost("CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("UpdateProduct/{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] long Id, [FromForm] UpdateProductCommand command)
    {
        if (command != null)
        {
            command.Id = Id;
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("GetAllProducts")]
    public async Task<ActionResult> GetAllProducts([FromQuery] string searchTerm)
    {
        var query = new GetAllProductsQuery { SearchTerm = searchTerm };
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("GetProduct/{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] long id)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("GetByBusiness")]
    public async Task<ActionResult> GetProductsByBusiness([FromQuery] string businessName)
    {
        var query = new GetProductsByBusinessQuery { BusinessName = businessName };
        var products = await mediator.Send(query);

        return Ok(products);
    }

    [HttpGet("GetByCategoryName")]
    public async Task<ActionResult> GetProductsByCategoryName([FromQuery] string categoryName)
    {
        var query = new GetProductsByCategoryNameQuery { CategoryName = categoryName };
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("GetByprice-range")]
    public async Task<ActionResult<List<ProductModel>>> GetProductsByPriceRange([FromQuery] GetProductsByPriceRangeQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}/products")]
    public async Task<ActionResult<List<ProductModel>>> GetProductsByUserId(string userId)
    {
        var query = new GetProductsByUserIdQuery { UserId = userId };
        var products = await mediator.Send(query);
        return Ok(products);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
