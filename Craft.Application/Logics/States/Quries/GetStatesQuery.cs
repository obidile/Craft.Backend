using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.States.Quries;

public class GetStatesQuery : IRequest<string>
{
}
public class GetStatesQueryHandler : IRequestHandler<GetStatesQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetStatesQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetStatesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.States.AsNoTracking();

        var result = await query.OrderBy(x => x.Name).ProjectTo<StateModel>(_mapper.ConfigurationProvider).ToListAsync();

        return result.ToString();
    }
}