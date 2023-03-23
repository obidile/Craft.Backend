using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Craft.Application.Logics.Schools.Command;

public class UpdateSchoolCommand : IRequest<string>
{
    public long SchoolId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public long StateId { get; set; }
    public long UserId { get; set; }
}
public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpdateSchoolCommandHandler(IApplicationContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<string> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            return "Your user Id was not found";
        }

        if (user.AccountType != AccountTypeEnum.Admin)
        {
            return "You do not have permission to update a school";
        }

        var school = await _dbContext.Schools.FindAsync(request.SchoolId);
        if (school == null)
        {
            return "The specified school was not found";
        }

        var state = await _dbContext.States.FindAsync(request.StateId);
        if (state == null)
        {
            return "The specified state was not found.";
        }

        school.Name = request.Name;
        school.IsActive = request.IsActive;
        school.State = state;
        school.UpdatedBy = $"{user.FirstName} - {user.LastName} {user.MailAddress}";
        school.UpdateDate = DateTime.UtcNow;

        _dbContext.Schools.Update(school);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"{school.Name} was successfully updated ";
    }
}