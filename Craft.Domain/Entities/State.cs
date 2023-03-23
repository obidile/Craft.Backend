using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class State : BaseObject
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public Country Country { get; set; }
    public ICollection<Business> Businesses { get; set; }
}
