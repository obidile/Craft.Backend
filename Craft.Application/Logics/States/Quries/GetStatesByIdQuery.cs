using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.States.Quries;

public class GetStatesByIdQuery : IRequest<string>
{
    public GetStatesByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
public class GetStatesByIdQueryHandler : IRequestHandler<GetStatesByIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetStatesByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetStatesByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.States.AsNoTracking().Where(x => x.Id == request.Id).Include(x => x.Country).ProjectTo<StateModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        if (data == null)
        {
            return "No state with the specified ID was found.";
        }

        return data.ToString();
    }
}