using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Countries.Command;

public class UpdateCountryCommand : IRequest<string>
{
    public long CountryId { get; set; }
    public string Name { get; set; }
    public IFormFile FlagUpload { get; set; }
    public bool IsActive { get; set; }
    public string UpdatedBy { get; set; }
    public int UserId { get; set; }
}
public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpdateCountryCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
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

        var model = await _dbContext.Countries.FirstOrDefaultAsync(x => x.Id == request.CountryId);
        if (model == null)
        {
            return "Country not found";
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

        model.Name = request.Name;
        model.UpdatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}";
        model.IsActive = request.IsActive;
        model.UpdateDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.FlagUpload != null || request.FlagUpload?.Length > 0)
        {
            string folder = $"image/FlagLogo/{model.Id}";
            var fileName = Guid.NewGuid().ToString();
            var filePath = Path.Combine($"wwwroot/{folder}", fileName);
            await UploadHelper.UploadFile(request.FlagUpload, filePath);
            model.FlagUrl = $"/{folder}/{fileName}";

            _dbContext.Countries.Update(model);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            return "An error occoured, Please try again.";
        }

        return "Country was successfully updated";
    }
}