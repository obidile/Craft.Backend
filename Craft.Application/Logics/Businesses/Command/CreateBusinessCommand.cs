using Craft.Application.Common.Helpers;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using FluentValidation;
using Serilog;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Craft.Domain.Enums;

namespace Craft.Application.Logics.Businesses.Command;

public class CreateBusinessCommand : IRequest<string>
{
    public string BusinessName { get; set; }
    public string BusinessAddress { get; set; }
    public IFormFile UploadLogo { get; set; }
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
        RuleFor(x => x.UploadLogo).NotNull().Must(x => x.Length > 0 && x.Length <= 5 * 1024 * 1024).WithMessage("The logo must be between 0 and 5 MB in size.");
    }
}
public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly ILogger<CreateBusinessCommand> _logger;
    public CreateBusinessCommandHandler(IApplicationContext dbContext, ILogger<CreateBusinessCommand> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
    {
        try
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

            if (request.UploadLogo != null || request.UploadLogo?.Length > 0)
            {
                string folder = $"image/BusinessLogos/{model.Id}";
                var fileName = Guid.NewGuid().ToString();
                var filePath = Path.Combine($"wwwroot/{folder}", fileName);
                await UploadHelper.UploadFile(request.UploadLogo, filePath);
            }

            model = await _dbContext.Businesses.AsNoTracking().Include(x => x.Country).Include(x => x.State).FirstOrDefaultAsync(x => x.Id == model.Id);

            _dbContext.Businesses.Update(model);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return "Business was successfully created";
        }
        catch (ValidationException ex)
        {
            return $"Validation failed: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the business");

            // Handle other errors
            return "An error occurred while creating the business";
        }


    }
     
}