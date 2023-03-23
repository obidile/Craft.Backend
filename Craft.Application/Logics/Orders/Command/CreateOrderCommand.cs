using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Craft.Application.Logics.Orders.Command;

public class CreateOrderCommand : IRequest<string>
{
    public List<OrderItem> OrderItems { get; set; }
    public string DeliveryAddress { get; set; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateOrderCommandHandler(IApplicationContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            OrderStatus = OrderStatus.Processing,
            DeliveryAddress = request.DeliveryAddress,
            PaymentDate = DateTime.Now,
            BusinessId = request.OrderItems.FirstOrDefault()?.Product.Id ?? 0, //BusinessId is always set to a valid value.
            OrderItems = request.OrderItems,
            CreatedDate = DateTime.UtcNow
        };

        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

        if (!string.IsNullOrEmpty(userId))
        {
            order.UserId = Convert.ToInt64(userId);
        }
        else
        {
            order.AnonymousId = Guid.NewGuid().ToString();
        }

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return "Your order has been placed";
    }
}

