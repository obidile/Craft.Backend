using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using Craft.Application.Logics.Users.Quries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Businesses.Quries;

public class GetBusinessesQuery : IRequest<List<BusinessModel>>
{
    public string BusinessName { get; set; }
    public string BusinessMail { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}
public class GetBusinessesQueryHandler : IRequestHandler<GetBusinessesQuery, List<BusinessModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetBusinessesQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<BusinessModel>> Handle(GetBusinessesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Businesses.AsNoTracking();
        query = query.Where(x => x.BusinessName.ToLower() == request.BusinessName.ToLower() || x.BusinessMail.ToLower() == request.BusinessMail.ToLower());

        // Paginate the result
        var pageSize = request.PageSize ?? 10;
        var pageNumber = request.PageNumber ?? 1;
        var totalRecords = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        var recordsToSkip = (pageNumber - 1) * pageSize;

        // Apply pagination to the query
        query = query.Skip(recordsToSkip).Take(pageSize);

        var result = await query.ProjectTo<BusinessModel>(_mapper.ConfigurationProvider).ToListAsync();
        var pagedResult = new PagedResult<BusinessModel>
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
    public List<BusinessModel> Results { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
}