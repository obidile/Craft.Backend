using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Schools.Command;

public class CreateSchoolCommand : IRequest<string>
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public long StateId { get; set; }
    public long UserId { get; set; }
}
public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateSchoolCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<string> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "Your user Id was not found";
        }

        if (user.AccountType != AccountTypeEnum.Admin)
        {
            return "You do not have permission to create a school";
        }

        var state = await _dbContext.States.FindAsync(request.StateId);
        if (state == null)
        {
            return "The specified state was not found";
        }

        var exist = await _dbContext.Schools.AsNoTracking().AnyAsync(x => x.Name.ToLower() == request.Name.ToLower());
        if (exist)
        {
            return "School already exists";
        }

        var model = new School()
        {
            Name = request.Name,
            IsActive = request.IsActive,
            State = state,
            CreatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}",
            CreatedDate = DateTime.UtcNow
        };

        await _dbContext.Schools.AddAsync(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"{model.Name} was successfully created ";
    }
}