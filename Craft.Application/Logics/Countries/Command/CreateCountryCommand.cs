using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;


namespace Craft.Application.Logics.Countries.Command;

public class CreateCountryCommand : IRequest<string>
{
    public string Name { get; set; }
    public IFormFile FlagUrl { get; set; }
    public bool IsActive { get; set; }
    //public int UserId { get; set; }
}
public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IConfiguration configuration;
    public CreateCountryCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
        this.configuration = configuration;
    }

    public async Task<string> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var jwt = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(jwt))
        {
            return "User has not been authenticated";
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(jwt);

        if (jwtToken == null)
        {
            return "Invalid JWT token";
        }

        var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return "User ID claim is missing";
        }

        var user = await _dbContext.Users.FindAsync(userId);

        if (user == null)
        {
            return "User not found in the database";
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
        
        var model = new Country()
        {
            Name = request.Name.ToUpper(),
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            IsActive = request.IsActive,
            CreatedDate = DateTime.UtcNow
        };
        _dbContext.Countries.Add(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.FlagUrl != null || request.FlagUrl?.Length > 0)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "images", "FlagLogo", model.Id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.FlagUrl.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            await UploadHelper.UploadFile(request.FlagUrl, fileName, folderPath);
            model.FlagUrl = Path.Combine("images", "FlagLogo", model.Id.ToString(), fileName);

            _dbContext.Countries.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return "Country was successfully created";
    }

}