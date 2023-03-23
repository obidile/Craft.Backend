using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class Product : BaseObject
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImageUrl { get; set; }
    public Business Business { get; set; }
    public Category Category { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Rating> Ratings { get; set; }
}
