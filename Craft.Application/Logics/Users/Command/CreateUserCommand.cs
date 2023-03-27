using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using CryptoHelper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Craft.Application.Logics.Users.Command;

public class CreateUserCommand : IRequest<string>
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public AccountTypeEnum AccountType { get; set; }
    public IFormFile UserImageUpload { get; set; }
    public bool IsActive { get; set; }
    public bool Deactivated { get; set; }
    public string Password { get; set; }
}
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{

    private readonly IApplicationContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IConfiguration config;
    public CreateUserCommandHandler(IApplicationContext dbContext, IEmailService emailService, IWebHostEnvironment hostEnvironment, IConfiguration config)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _hostEnvironment = hostEnvironment;
        this.config = config;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountType == AccountTypeEnum.Admin)
        {
            int adminCount = await _dbContext.Users.CountAsync(x => x.AccountType == AccountTypeEnum.Admin);
           var MaxAdminUsers = config.GetValue<int>("EmailSettings:SmtpPort");
            if (adminCount >= MaxAdminUsers)
            {
                return "Cannot create new admin user - maximum number of admin users reached";
            }
        }

        if (!new EmailAddressAttribute().IsValid(request.MailAddress))
        {
            return "Invalid email format";
        }

        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.MailAddress.ToLower() == request.MailAddress.ToLower(), cancellationToken);
        if (user != null)
        {
            return "This User already Exist";
        }

        var accountTypeString = request.AccountType.ToString();

        var model = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            MailAddress = request.MailAddress,
            AccountType = Enum.Parse<AccountTypeEnum>(accountTypeString),
            IsActive = true,
            Deactivated = false,
            PasswordHush = Crypto.HashPassword(request.Password),
            CreatedDate = DateTime.UtcNow,
        };

        _dbContext.Users.Add(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.UserImageUpload != null || request.UserImageUpload?.Length > 0)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "images", "UsersImages", model.Id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.UserImageUpload.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            await UploadHelper.UploadFile(request.UserImageUpload, fileName, folderPath);
            model.ImageUrl = Path.Combine("images", "UsersImages", model.Id.ToString(), fileName);

            _dbContext.Users.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Send email to new user
        var emailSubject = "Welcome to Craft application!";
        var emailBody =
            $"Dear {model.FirstName} {model.LastName},<br><br>" +
            $"Thank you for signing up in our application." +
            $" We look forward to giving you a wonderful exprince!";
        await _emailService.SendEmail(model.MailAddress, emailSubject, emailBody);

        return "User successfully created";
    }


}