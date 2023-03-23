using Craft.Application.Common.Enums;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class OrderModel: BaseModel
{
    public string AnonymousId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string DeliveryAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime PaymentDate { get; set; }

    // Registered User Information
    public long? UserId { get; set; }
    public User User { get; set; }

    // Business Information
    public long BusinessId { get; set; }
    public Business Business { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}
