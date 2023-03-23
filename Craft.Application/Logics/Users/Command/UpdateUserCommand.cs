using AutoMapper;
using Craft.Application.Common.Enums;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Craft.Application.Logics.Users.Command;

public class UpdateUserCommand : IRequest<string>
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public IFormFile UserImageUpload { get; set; }
    public bool IsActive { get; set; }
    public bool Deactivated { get; set; }
    public string PasswordHush { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.BeginTransactionAsync(cancellationToken);

        try
        {
            if (!new EmailAddressAttribute().IsValid(request.MailAddress))
            {
                return "Invalid email format";
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (user == null)
            {
                return "User was not found";
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.MailAddress = request.MailAddress;
            user.PhoneNumber = request.PhoneNumber;
            user.Deactivated = request.Deactivated;
            user.IsActive = request.IsActive;
            user.UpdateDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (request.UserImageUpload != null || request.UserImageUpload?.Length > 0)
            {
                string folder = $"image/UsersImages/{user.Id}";
                var fileName = Guid.NewGuid().ToString();
                var filePath = Path.Combine($"wwwroot/{folder}", fileName);
                await UploadHelper.UploadFile(request.UserImageUpload, filePath);

                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return "User was successfully updated";
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new NotImplementedException("Error try again");
        }
    }
}
