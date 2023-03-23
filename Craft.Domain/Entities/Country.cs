using Craft.Domain.Common;

namespace Craft.Domain.Entities;

public class Country : BaseObject
{
    public string Name { get; set; }
    public string FlagUrl { get; set; }
    public bool IsActive { get; set; }
    public ICollection<State> States { get; set; }
}
