using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class OrderItemModel : BaseModel
{
    public int OrderId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
