using Craft.Domain.Common;
using System.Diagnostics.Metrics;

namespace Craft.Domain.Entities;

public class Business : BaseObject
{
    public long UserId { get; set; }
    public string BusinessName { get; set; }
    public string BusinessAddress { get; set; }
    public string Logo { get; set; }
    public string BusinessMail { get; set; }
    public string PhoneNumber { get; set; }
    public string DisplayedPhoneNumber { get; set; }
    public long CountryId { get; set; }
    public Country Country { get; set; }
    public long? StateId { get; set; }
    public State State { get; set; }
    public bool IsActive { get; set; }
    public string BankName { get; set; }
    public string BankAccountName { get; set; }
    public string AccountNumber { get; set; }
    public string WebsiteURL { get; set; }
    public string InstgramURL { get; set; }
    public string TwitterURL { get; set; }
    public string LinkedinUrl { get; set; }

    public ICollection<Rating> Ratings { get; set; }

}
