using Craft.Application.Common.Mappers;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class SchoolModel : BaseModel, IMapFrom<School>
{
    public long StateId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public State State { get; set; }
}
