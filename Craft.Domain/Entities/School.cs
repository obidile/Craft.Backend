using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class School : BaseObject
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public State State { get; set; }
    public ICollection<Business> Businesses { get; set; }
}