using Craft.Application.Common.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Craft.Application.Logics.Products.Command;

public class DeleteProductCommand : IRequest<string>
{
    public long Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteProductCommandHandler(IApplicationContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "Your user Id was not found";
        }

        var product = await _dbContext.Products.FindAsync(request.Id);
        if (product == null)
        {
            return "Product not found";
        }

        if (product.Business.UserId.ToString() != userId)
        {
            return "You do not have permission to delete this product";
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return "Product was successfully deleted";
    }
}
