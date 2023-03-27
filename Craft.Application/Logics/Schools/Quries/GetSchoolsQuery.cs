using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Schools.Quries;

public class GetSchoolsQuery : IRequest<string>
{
}
public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetSchoolsQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<string> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var query = await _dbContext.Schools.AsNoTracking().OrderBy(x => x.Name).ProjectTo<SchoolModel>(_mapper.ConfigurationProvider).ToListAsync();

        return query.ToString();
    }
}