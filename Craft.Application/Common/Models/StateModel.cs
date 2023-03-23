using Craft.Application.Common.Mappers;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class StateModel : BaseModel, IMapFrom<State>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public State State { get; set; }
    public ICollection<Business> Businesses { get; set; }
}
