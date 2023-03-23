using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Craft.Application.Common.Interfaces;

public class ApplicationContext : DbContext, IApplicationContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
