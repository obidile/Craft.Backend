using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using CryptoHelper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Craft.Application.Logics.PasswordManagement;

public class ChangePasswordCommand : IRequest<string>
{
    public string MailAddress { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public long UserId { get; set; }

}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ChangePasswordCommandHandler(IApplicationContext dbContext, IMediator mediator, IMapper mapper)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<string> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.MailAddress.ToLower() == request.MailAddress || x.Id == request.UserId);

        if (user == null)
        {
            return "User was not found";
        }

        if (user != null)
        {
            string hashedPassword = user.PasswordHush;

            string oldPasswordHash = Crypto.HashPassword(request.OldPassword);

            if (hashedPassword != oldPasswordHash)
            {
                return "Old password is incorrect";
            }

            if (request.NewPassword.Length < 8 || !Regex.IsMatch(request.NewPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"))
            {
                return "New password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
            }

            if (request.NewPassword.ToLower().Equals(request.OldPassword.ToLower()))
            {
                return "old and new password cannot be the same";
            }

            bool Space = request.NewPassword.Contains(" ");
            if (Space)
            {
                return "New Password cannot contain spaces";
            }

            user.IsActive = true;
            Crypto.HashPassword(request.NewPassword);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        return "Your password was changed successfully";
    }
}