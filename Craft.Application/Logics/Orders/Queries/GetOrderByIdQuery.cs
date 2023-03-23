using Craft.Application.Common.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Orders.Queries;

public class GetOrderByIdQuery : IRequest<string>
{
    public GetOrderByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    public GetOrderByIdHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (order == null)
        {
            return $"Order {request.Id} not found";
        }

        return order.ToString();
    }
}