using AutoMapper;
using Craft.Application.Common.Enums;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    private readonly IWebHostEnvironment _hostEnvironment;

    public UpdateUserCommandHandler(IApplicationContext dbContext, IWebHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        

            if (!new EmailAddressAttribute().IsValid(request.MailAddress))
            {
                return "Invalid email format";
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

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
            string webRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "images", "UsersImages", user.Id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.UserImageUpload.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            await UploadHelper.UploadFile(request.UserImageUpload, fileName, folderPath);
            user.ImageUrl = Path.Combine("images", "UsersImages", user.Id.ToString(), fileName);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        return "User was successfully updated";
    }
}
