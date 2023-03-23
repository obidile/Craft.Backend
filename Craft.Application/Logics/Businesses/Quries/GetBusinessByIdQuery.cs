using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Businesses.Quries;

public class GetBusinessByIdQuery : IRequest<BusinessModel>
{
    public GetBusinessByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQuery, BusinessModel>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetBusinessByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<BusinessModel> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Businesses.AsNoTracking().Where(x => x.Id == request.Id).ProjectTo<BusinessModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);
        if (data == null)
        {
            throw new Exception("No Business with the specified ID was found.");
        }

        return data;
    }
}