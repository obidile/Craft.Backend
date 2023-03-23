using Craft.Domain.Entities;
using MXTires.Microdata.Core.Intangible;

namespace Craft.Application.Common.Models;

public class RatingModel : BaseModel
{
    public int Value { get; set; }
    public string Comment { get; set; }
    public User User { get; set; }
    public Business Business { get; set; }
    public Product Product { get; set; }
}
