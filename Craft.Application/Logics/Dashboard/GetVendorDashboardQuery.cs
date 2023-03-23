using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Logics.Dashboard;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Dashboard;

public class GetVendorDashboardQuery : IRequest<string>
{
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalProductsSold { get; set; }
    public int TotalBusinessesOwned { get; set; }
    public int TotalRatingsReceived { get; set; }
}
public class GetVendorDashboardQueryHandler : IRequestHandler<GetVendorDashboardQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetVendorDashboardQueryHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(GetVendorDashboardQuery request, CancellationToken cancellationToken)
    {
       var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return "User Id not found in claims.";
        }

        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "User not found in database.";
        }

        var fulfilledOrders = await _dbContext.Orders.Include(x => x.OrderItems).Where(x => x.OrderStatus == OrderStatus.Fulfilled).ToListAsync(cancellationToken);

        var vendorOrderItems = fulfilledOrders.SelectMany(x => x.OrderItems).Where(x => x.Product.Id.ToString() == userId).ToList();

        var totalSales = vendorOrderItems.Sum(x => x.Quantity);
        var totalRevenue = vendorOrderItems.Sum(x => x.Quantity * x.Product.Price);
        var totalProductsSold = vendorOrderItems.Count;
        var totalBusinessesOwned = await _dbContext.Businesses.CountAsync(x => x.UserId.ToString() == userId);
        var totalRatingsReceived = await _dbContext.Ratings.CountAsync(x => x.User.Id.ToString() == userId);

        var result = new GetVendorDashboardQuery
        {
            TotalSales = totalSales,
            TotalRevenue = totalRevenue,
            TotalProductsSold = totalProductsSold,
            TotalBusinessesOwned = totalBusinessesOwned,
            TotalRatingsReceived = totalRatingsReceived
        };

        return result.ToString();
    }
}