using Craft.Application.Common.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.OrderItems.Quries;

public class GetOrderItemsByOrderIdQuery : IRequest<string>
{
    public GetOrderItemsByOrderIdQuery(long orderId)
    {
        OrderId = orderId;
    }

    public long OrderId { get; set; }
}
public class GetOrderItemsByOrderIdQueryHandler : IRequestHandler<GetOrderItemsByOrderIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    public GetOrderItemsByOrderIdQueryHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(GetOrderItemsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var orderItems = await _dbContext.OrderItems.Where(x => x.OrderId == request.OrderId).ToListAsync();

        return orderItems.ToString();
    }
}