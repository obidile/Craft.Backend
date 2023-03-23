using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Users.Quries;

public class GetUsersQuery : IRequest<List<UserModel>>
{
    public string MailAddress { get; set; }
    public bool IsActive { get; set; }
    public bool Deactivated { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get;  set; }
}

public class UsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public UsersQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<UserModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(request.MailAddress))
        {
            query = query.Where(x => x.MailAddress.ToLower() == request.MailAddress.ToLower());
        }

        if (request.IsActive)
        {
            query = query.Where(x => x.IsActive == request.IsActive);
        }
        if (request.Deactivated)
        {
            query = query.Where(x => x.Deactivated == request.Deactivated);
        }

        // Paginate the result
        var pageSize = request.PageSize ?? 10;
        var pageNumber = request.PageNumber ?? 1;
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var recordsToSkip = (pageNumber - 1) * pageSize;

        // Apply pagination to the query
        query = query.Skip(recordsToSkip).Take(pageSize);

        var result = await query.ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
         var pagedResult = new PagedResult<UserModel>
         {
        Results = result,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPages = totalPages,
        TotalRecords = totalRecords
         };

        return pagedResult.Results;
    }
}

public class PagedResult<T>
{
    public List<UserModel> Results { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}