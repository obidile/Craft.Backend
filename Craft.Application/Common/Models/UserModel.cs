using Craft.Application.Common.Enums;
using Craft.Application.Common.Mappers;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Models;

public class UserModel : BaseModel, IMapFrom<User>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfilePic { get; set; }
    public bool IsActive { get; set; }
    public bool Deactivated { get; set; }
    public string PasswordHush { get; set; }

    public ICollection<Business> Businesses { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Rating> Ratings { get; set; }
}
