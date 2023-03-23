using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Orders.Queries;

public class GetOrdersPlacedOneHourAgoQuery : IRequest<List<Order>>
{
}
public class GetOrdersPlacedOneHourAgoQueryHandler : IRequestHandler<GetOrdersPlacedOneHourAgoQuery, List<Order>>
{
    private readonly IApplicationContext _dbContext;
    public GetOrdersPlacedOneHourAgoQueryHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    } 
    public async Task<List<Order>> Handle(GetOrdersPlacedOneHourAgoQuery request, CancellationToken cancellationToken)
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1);

        var orders = await _dbContext.Orders.Where(x => x.CreatedDate >= oneHourAgo && x.CreatedDate < DateTime.UtcNow).ToListAsync(cancellationToken);

        return orders;
    }
}