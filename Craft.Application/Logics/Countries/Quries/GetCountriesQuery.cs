using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Countries.Quries;

public class GetCountriesQuery : IRequest<string>
{
    public string Name { get; set; }
}
public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetCountriesQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Countries.AsNoTracking();
        query = query.Where(x => x.Name.ToLower() == request.Name.ToLower());

        var list = await query.OrderBy(x => x.Name).ToListAsync();

        var result = _mapper.Map<List<CountryModel>>(list);
        return result.ToString();
    }
}