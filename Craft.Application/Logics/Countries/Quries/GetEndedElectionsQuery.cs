using AutoMapper;
using Craft.Application.Common.Interface;
using MediatR;

namespace Craft.Application.Logics.Countries.Quries;

public class GetEndedElectionsQuery : IRequest<string>
{
}
public class GetEndedElectionsQueryHandler : IRequestHandler<GetEndedElectionsQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetEndedElectionsQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<string> Handle(GetEndedElectionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}