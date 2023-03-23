using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Dashboard;

public class GetAdminDashboardQuery : IRequest<string>
{
    public int TotalCustomers { get; set; }
    public int TotalVendors { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int FulfilledOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalBusinesses { get; set; }
    public int TotalCountries { get; set; }
    public int TotalStates { get; set; }
    public int TotalSchools { get; set; }
    public int TotalRatings { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int DeactivatedUsers { get; set; }
    public int NewBusinesses { get; set; }
    public int TotalNewOrders { get; set; }
}
public class DashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public DashboardQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
            request.TotalCustomers = await _dbContext.Users.CountAsync(x => x.AccountType == AccountTypeEnum.Customer);
            request.TotalVendors = await _dbContext.Users.CountAsync(x => x.AccountType == AccountTypeEnum.Vendor);
            request.TotalOrders = await _dbContext.Orders.CountAsync();
            request.PendingOrders = await _dbContext.Orders.CountAsync(x => x.OrderStatus == OrderStatus.Pending);
            request.TotalBusinesses = await _dbContext.Businesses.CountAsync();
            request.TotalProducts = await _dbContext.Products.CountAsync();
            request.TotalCountries = await _dbContext.Countries.CountAsync();
            request.TotalStates = await _dbContext.States.CountAsync();
            request.TotalSchools = await _dbContext.Schools.CountAsync();
            request.TotalRatings = await _dbContext.Ratings.CountAsync();
            request.TotalAdmins = await _dbContext.Users.CountAsync(x => x.AccountType == AccountTypeEnum.Admin);
            request.TotalUsers = await _dbContext.Users.CountAsync();
            request.ActiveUsers = await _dbContext.Users.CountAsync(x => x.IsActive);
            request.DeactivatedUsers = await _dbContext.Users.CountAsync(x => x.Deactivated);
            request.NewBusinesses = await _dbContext.Businesses.CountAsync(x => x.CreatedDate >= DateTime.Today.AddDays(-7));
            request.TotalNewOrders = await _dbContext.Orders.CountAsync(x => x.CreatedDate >= DateTime.Today.AddDays(-2));


        var result = new GetAdminDashboardQuery()
        {
            TotalCustomers = request.TotalCustomers,
            TotalVendors = request.TotalVendors,
            TotalOrders = request.TotalOrders,
            TotalBusinesses = request.TotalBusinesses,
            PendingOrders = request.PendingOrders,
            TotalProducts = request.TotalProducts,
            TotalCountries = request.TotalCountries,
            TotalStates = request.TotalStates,
            TotalSchools = request.TotalSchools,
            TotalRatings = request.TotalRatings,
            TotalAdmins = request.TotalAdmins,
            TotalUsers = request.TotalUsers,
            ActiveUsers = request.ActiveUsers,
            DeactivatedUsers = request.DeactivatedUsers,
            NewBusinesses = request.NewBusinesses,
            TotalNewOrders = request.TotalNewOrders,
            };

            return result.ToString();
    }
}