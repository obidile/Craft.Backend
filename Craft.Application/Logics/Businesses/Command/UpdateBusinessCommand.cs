using AutoMapper;
using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Craft.Application.Logics.Businesses.Command;

public class UpdateBusinessCommand : IRequest<string>
{
    public long Id { get; set; }
    public string BusinessName { get; set; }
    public string BusinessAddress { get; set; }
    public IFormFile LogoUrl { get; set; }
    public string BusinessMail { get; set; }
    public string PhoneNumber { get; set; }
    public string DisplayedPhoneNumber { get; set; }
    public long CountryId { get; set; }
    public long? StateId { get; set; }
    public bool IsActive { get; set; }
    public string BankName { get; set; }
    public string BankAccountName { get; set; }
    public string AccountNumber { get; set; }
    public string WebsiteURL { get; set; }
    public string InstgramURL { get; set; }
    public string TwitterURL { get; set; }
    public string LinkedinUrl { get; set; }

    public ICollection<Rating> Ratings { get; set; }
}
public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _hostEnvironment;
    public UpdateBusinessCommandHandler(IApplicationContext dbContext, IMapper mapper, IWebHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string> Handle(UpdateBusinessCommand request, CancellationToken cancellationToken)
    {
        var business = await _dbContext.Businesses.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (business == null)
        {
            return "User is not found";
        }

        business.BusinessName = request.BusinessName;
        business.BusinessAddress = request.BusinessAddress;
        business.BusinessMail = request.BusinessMail;
        business.PhoneNumber = request.PhoneNumber;
        business.DisplayedPhoneNumber = request.DisplayedPhoneNumber;
        business.CountryId = request.CountryId;
        business.StateId = request.StateId;
        business.IsActive = request.IsActive;
        business.BankName = request.BankName;
        business.BankAccountName = request.BankAccountName;
        business.AccountNumber = request.AccountNumber;
        business.WebsiteURL = request.WebsiteURL;
        business.InstgramURL = request.InstgramURL;
        business.TwitterURL = request.TwitterURL;
        business.LinkedinUrl = request.LinkedinUrl;
        business.CreatedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);


        if (request.LogoUrl != null || request.LogoUrl?.Length > 0)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "images", "BusinessLogos", business.Id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LogoUrl.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            await UploadHelper.UploadFile(request.LogoUrl, fileName, folderPath);
            business.Logo = Path.Combine("images", "BusinessLogos", business.Id.ToString(), fileName);

            _dbContext.Businesses.Update(business);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return "Business Profile was successfully updated";
    }
}