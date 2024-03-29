﻿using Craft.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Common.Interface;

public interface IApplicationContext
{
    DbSet<User> Users { get; set; }
    DbSet<Business> Businesses { get; set; }
    DbSet<Rating> Ratings { get; set; }
    DbSet<Country> Countries { get; set; }
    DbSet<Category> Categories { get; set; }
    DbSet<State> States { get; set; }
    DbSet<School> Schools { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }




    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
   
}
