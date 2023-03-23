using Craft.Application.Common.Interface;
using Craft.Domain.Common;
using Craft.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MXTires.Microdata.Core.Intangible;

namespace Craft.Application.Logics.Ratings;

public class CreateRatingCommand : IRequest<string>
{
    public int Value { get; set; }
    public string Comment { get; set; }
    public long UserId { get; set; }
    public long? BusinessId { get; set; }
    public long? ProductId { get; set; }
}
public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, string>
{
    private readonly IApplicationContext _dbContext;
    public CreateRatingCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(request.UserId);
        if (user == null)
        {
            return "Your user ID was not found";
        }

        if (request.BusinessId == null && request.ProductId == null)
        {
            return "You must specify either a business or a product to rate";
        }

        string ratedItem;
        Object ratedObject;

        if (request.BusinessId != null)
        {
            var business = await _dbContext.Businesses.FindAsync(request.BusinessId);
            if (business == null)
            {
                return "The specified business was not found";
            }
            ratedObject = business;
            ratedItem = "business";
        }
        else
        {
            var product = await _dbContext.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return "The specified product was not found";
            }

            ratedObject = product;
            ratedItem = "product";
        }

        if (request.Value < 1 || request.Value > 5)
        {
            return "The rating value must be between 1 and 5";
        }

        var model = new Domain.Entities.Rating()
        {
            Value = request.Value,
            Comment = request.Comment,
            User = user,
            Business = ratedObject as Business,
            Product = ratedObject as Product,
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            CreatedDate = DateTime.UtcNow
        };

        await _dbContext.Ratings.AddAsync(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"Your {ratedItem} review was successfully, Thank you";
    }
}