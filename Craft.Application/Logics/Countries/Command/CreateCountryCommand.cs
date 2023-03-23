using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Craft.Application.Logics.Countries.Command;

public class CreateCountryCommand : IRequest<string>
{
    public string Name { get; set; }
    public IFormFile FlagUpload { get; set; }
    public bool IsActive { get; set; }
    public int UserId { get; set; }
}
public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateCountryCommandHandler(IApplicationContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<string> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "Your user Id was not found";
        }

        if (user.AccountType != AccountTypeEnum.Admin)
        {
            return "You do not have permission to create a country";
        }

        var exist = await _dbContext.Countries.AsNoTracking().AnyAsync(x => x.Name.ToLower() == request.Name.ToLower());
        if (exist)
        {
            return "Country name already exist";
        }

        if (request.IsActive != true && request.IsActive != false)
        {
            return "Invalid IsActive parameter value";
        }

        if (request.FlagUpload != null)
        {
            if (request.FlagUpload.Length <= 0 || request.FlagUpload.Length > 5242880)
            {
                return "Invalid flag file size";
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(request.FlagUpload.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid flag file type";
            }
        }

        var model = new Country()
        {
            Name = request.Name.ToUpper(),
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            IsActive = request.IsActive,
            CreatedDate = DateTime.UtcNow
        };
        _dbContext.Countries.Add(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.FlagUpload != null || request.FlagUpload?.Length > 0)
        {
            string folder = $"image/FlagLogo/{model.Id}";
            var fileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine($"wwwroot/{folder}", fileName);

            // Check if model.Id property is null or empty
            if (string.IsNullOrEmpty(model.Id.ToString()))
            {
                return "Error: Invalid model.Id value";
            }

            await UploadHelper.UploadFile(request.FlagUpload, filePath);
            model.FlagUrl = $"/{folder}/{fileName}";

            _dbContext.Countries.Update(model);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            return "An error occoured while uploading flag, Please try again.";
        }

        return "Country was successfully created";
    }

}