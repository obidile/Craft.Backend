using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;

namespace Craft.Application.Logics.Orders.Command;

public class CancelOrderCommand : IRequest<string>
{
    public long OrderId { get; set; }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, string>
{
    private readonly IApplicationContext _dbContext;

    public CancelOrderCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);

        if (order == null)
        {
            return "Order not found";
        }

        if (DateTime.Now - order.CreatedDate > TimeSpan.FromHours(1))
        {
            return "Order cannot be cancelled because it was placed more than 1 hour ago.";
        }

        order.OrderStatus = OrderStatus.Cancelled;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"Order {request.OrderId} has been cancelled.";
    }
}

