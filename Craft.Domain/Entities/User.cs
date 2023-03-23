using Craft.Domain.Common;
using Craft.Domain.Enums;

namespace Craft.Domain.Entities;

public class User : BaseObject
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public AccountTypeEnum AccountType { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string VerificationCode { get; set; }
    public bool IsVerified { get; set; }
    public bool Deactivated { get; set; }
    public string PasswordHush { get; set; }
    public ICollection<Business> Businesses { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Rating> Ratings { get; set; }
}