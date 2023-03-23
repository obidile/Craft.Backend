using Craft.Application.Common.Mappers;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class CountryModel : BaseModel, IMapFrom<Country>
{
    public string Name { get; set; }
    public string FlagUrl { get; set; }
    public bool IsActive { get; set; }
    public ICollection<State> States { get; set; }
}
