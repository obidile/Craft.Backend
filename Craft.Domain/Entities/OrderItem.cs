using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class OrderItem : BaseObject
{
    public int OrderId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
