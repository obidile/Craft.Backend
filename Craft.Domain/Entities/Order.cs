using Craft.Domain.Common;
using Craft.Domain.Enums;

namespace Craft.Domain.Entities;

public class Order : BaseObject
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
