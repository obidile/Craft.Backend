using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Craft.Application.Logics.Businesses.Command;

public class CreateBusinessCommand : IRequest<string>
{
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
public class CreateBusinessCommandValidator : AbstractValidator<CreateBusinessCommand>
{
    public CreateBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BusinessAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BusinessMail).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\d{11}$");
        RuleFor(x => x.DisplayedPhoneNumber).NotEmpty().Matches(@"^\d{11}$");
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.StateId).NotEmpty();
        RuleFor(x => x.BankName).MaximumLength(50);
        RuleFor(x => x.BankAccountName).MaximumLength(50);
        RuleFor(x => x.AccountNumber).MaximumLength(50);
        RuleFor(x => x.WebsiteURL).MaximumLength(100);
        RuleFor(x => x.InstgramURL).MaximumLength(100);
        RuleFor(x => x.TwitterURL).MaximumLength(100);
        RuleFor(x => x.LinkedinUrl).MaximumLength(100);
        RuleFor(x => x.LogoUrl).NotNull().Must(x => x.Length > 0 && x.Length <= 5 * 1024 * 1024).WithMessage("The logo must be between 0 and 5 MB in size.");
    }
}
public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IWebHostEnvironment _hostEnvironment;
    public CreateBusinessCommandHandler(IApplicationContext dbContext, IWebHostEnvironment hostEnvironment)
    {
        _dbContext = dbContext;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<string> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
    {
            var exist = await _dbContext.Businesses.AsNoTracking().AnyAsync(x => x.BusinessName.ToLower() == request.BusinessName.ToLower());
            if (exist)
            {
                return $"{request.BusinessName} already exist";
            }

            var model = new Business()
            {
                BusinessName = request.BusinessName,
                BusinessAddress = request.BusinessAddress,
                BusinessMail = request.BusinessMail,
                PhoneNumber = request.PhoneNumber,
                DisplayedPhoneNumber = request.DisplayedPhoneNumber,
                CountryId = request.CountryId,
                StateId = request.StateId,
                IsActive = request.IsActive,
                BankName = request.BankName,
                BankAccountName = request.BankAccountName,
                AccountNumber = request.AccountNumber,
                WebsiteURL = request.WebsiteURL,
                InstgramURL = request.InstgramURL,
                TwitterURL = request.TwitterURL,
                LinkedinUrl = request.LinkedinUrl,
                CreatedDate = DateTime.UtcNow
            };
            _dbContext.Businesses.Add(model);
            await _dbContext.SaveChangesAsync(cancellationToken);


        if (request.LogoUrl != null || request.LogoUrl?.Length > 0)
        {
            string webRootPath = _hostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "images", "BusinessLogos", model.Id.ToString());

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LogoUrl.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            await UploadHelper.UploadFile(request.LogoUrl, fileName, folderPath);
            model.Logo = Path.Combine("images", "BusinessLogos", model.Id.ToString(), fileName);

            _dbContext.Businesses.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        model = await _dbContext.Businesses.AsNoTracking().Include(x => x.Country).Include(x => x.State).FirstOrDefaultAsync(x => x.Id == model.Id);

            _dbContext.Businesses.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return "Business profile was successfully created";


    }
     
}