using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using CryptoHelper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(IApplicationContext dbContext, IMapper mapper, IEmailService emailService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

            if (!new EmailAddressAttribute().IsValid(request.MailAddress))
            {
                return "Invalid email format";
            }

            var user = await _dbContext.Users.AsNoTracking().AnyAsync(x => x.MailAddress.ToLower() == request.MailAddress.ToLower());
            if (user)
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
                string folder = $"image/UsersImages/{model.Id}";
                var fileName = Guid.NewGuid().ToString();
                var filePath = Path.Combine($"wwwroot/{folder}", fileName);
                await UploadHelper.UploadFile(request.UserImageUpload, filePath);
                model.ImageUrl = request.UserImageUpload.ToString();

                _dbContext.Users.Update(model);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            // Send email to new user
            var emailSubject = "Welcome to our application!";
            var emailBody =
                $"Dear {model.FirstName} {model.LastName},<br><br>" +
                $"Thank you for signing up in our application." +
                $" We look forward to giving you a wonderful exprince!";
            await _emailService.SendEmail(model.MailAddress, emailSubject, emailBody);

            return "User successfully created";
        }
        catch (Exception)
        {
            throw new NotImplementedException("Error try again");
        }
    }



}