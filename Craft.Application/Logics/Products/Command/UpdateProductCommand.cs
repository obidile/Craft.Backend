using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Products.Command;

public class UpdateProductCommand : IRequest<string>
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public IFormFile ImageUrlUpload { get; set; }
    public long BusinessId { get; set; }
    public long CategoryId { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProductCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "Your user Id was not found";
        }

        if (user.AccountType != AccountTypeEnum.Admin || user.AccountType != AccountTypeEnum.Vendor)
        {
            return "You do not have permission to create a product";
        }

        var product = await _dbContext.Products.Include(x => x.Business).Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (product == null)
        {
            return "The specified product could not be found";
        }

        if (product.Business.UserId.ToString() != userId)
        {
            return "You do not have permission to update this product";
        }

        var business = await _dbContext.Businesses.FindAsync(request.BusinessId);
        if (business == null)
        {
            return "Only after creating a business profile can you add products";
        }

        var category = await _dbContext.Categories.FindAsync(request.CategoryId);

        if (category == null)
        {
            return "Please select a valid category";
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.Business = business;
        product.Category = category;
        product.UpdatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}";
        product.UpdateDate = DateTime.UtcNow;

        if (request.ImageUrlUpload != null && request.ImageUrlUpload.Length > 0)
        {
            string folder = $"image/ProductImages/{product.Id}";
            var fileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine($"wwwroot/{folder}", fileName);

            try
            {
                await UploadHelper.UploadFile(request.ImageUrlUpload, filePath);
            }
            catch (Exception)
            {
                return $"An error occurred while uploading the product image, Please try again.";
            }

            product.ImageUrl = $"/{folder}/{fileName}";
        }

        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return "Product was successfully updated";
    }
}
