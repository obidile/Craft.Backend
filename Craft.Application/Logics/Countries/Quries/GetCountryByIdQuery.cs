using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Countries.Quries;

public class GetCountryByIdQuery : IRequest<string>
{
    public long Id { get; set; }
}
public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetCountryByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Countries.AsNoTracking().Where(x => x.Id == request.Id).ProjectTo<CountryModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        if (data == null)
        {
            throw new Exception("No Country with the specified ID was found.");
        }

        return data.ToString();
    }
}