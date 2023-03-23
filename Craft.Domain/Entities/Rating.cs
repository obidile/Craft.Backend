using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class Rating : BaseObject
{
    public int Value { get; set; }
    public string Comment { get; set; }
    public User User { get; set; }
    public Business Business { get; set; }
    public Product Product { get; set; }
}
