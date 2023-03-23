using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class DashboardModel
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

public class VendorDashboard : DashboardModel
{
    public int TotalSales { get; set; }
    public int TotalRevenue { get; set; }
    public int TotalProductsSold { get; set; }
    public int TotalBusinessesOwned { get; set; }
    public int TotalRatingsReceived { get; set; }
}