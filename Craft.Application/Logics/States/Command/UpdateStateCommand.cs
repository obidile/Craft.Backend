using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.States.Command;

public class UpdateStateCommand : IRequest<string>
{
    public long StateId { get; set; }
    public string Name { get; set; }
    public long CountryId { get; set; }
    public bool IsActive { get; set; }
    public long UserId { get; set; }
}
public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpdateStateCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<string> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
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


        var state = await _dbContext.States.FindAsync(request.StateId);
        if (state == null)
        {
            return "The specified state was not found";
        }

        var country = await _dbContext.Countries.FindAsync(request.CountryId);
        if (country == null)
        {
            return "The specified country was not found.";
        }

        state.Name = request.Name;
        state.IsActive = request.IsActive;
        state.Country = country;
        state.UpdatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}";
        state.UpdateDate = DateTime.UtcNow;

        _dbContext.States.Update(state);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"You have successfully updated {state.Name} state in {state.Country}";
    }
}