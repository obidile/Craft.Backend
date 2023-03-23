using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class Category : BaseObject
{
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}
