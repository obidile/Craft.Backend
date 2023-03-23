using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class CategoryModel : BaseModel
{
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}
