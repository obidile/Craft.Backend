using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.States.Command;

public class CreateStateCommand : IRequest<string>
{
    public string Name { get; set; }
    public long CountryId { get; set; }
    public bool IsActive { get; set; }
    public long UserId { get; set; }
}
public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateStateCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Handle(CreateStateCommand request, CancellationToken cancellationToken)
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

        var country = await _dbContext.Countries.FindAsync(request.CountryId);
        if (country == null)
        {
            return "The specified country was not found.";
        }

        var exist = await _dbContext.States.AsNoTracking().AnyAsync(x => x.Name.ToLower() == x.Name.ToLower());
        if (exist)
        {
            return "State already exist";
        }

        var model = new State()
        {
            Name = request.Name,
            IsActive = request.IsActive,
            Country = country,
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            CreatedDate = DateTime.UtcNow
        };

        await _dbContext.States.AddAsync(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"You have successfully created {model.Name} state in {model.Country}";
    }
}