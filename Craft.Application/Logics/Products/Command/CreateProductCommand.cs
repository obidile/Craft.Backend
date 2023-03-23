using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Products.Command;

public class CreateProductCommand : IRequest<string>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public IFormFile ImageUrlUpload { get; set; }
    public long BusinessId { get; set; }
    public long CategoryId { get; set; }
}
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateProductCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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

        var productWithSameName = await _dbContext.Products.Where(x => x.Name == request.Name && x.Business.UserId.ToString() == userId)
            .FirstOrDefaultAsync();

        if (productWithSameName != null)
        {
            return
                    $"{productWithSameName.Name} already exists as a product. " +
                    $"you can only add to its quantity as there is no need to add the same product more than once.";
        }

        var model = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            Business = business,
            Category = category,
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            CreatedDate = DateTime.UtcNow
        };

        _dbContext.Products.Add(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.ImageUrlUpload != null && request.ImageUrlUpload.Length > 0)
        {
            string folder = $"image/ProductImages/{model.Id}";
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

            model.ImageUrl = $"/{folder}/{fileName}";

            _dbContext.Products.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return "Product was successfully created";
    }

}