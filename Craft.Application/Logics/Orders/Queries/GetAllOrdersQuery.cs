using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Orders.Queries;

public class GetAllOrdersQuery : IRequest<string>
{
}
public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetAllOrdersQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Orders.AsNoTracking().OrderBy(x => x.CreatedDate).ProjectTo<OrderModel>(_mapper.ConfigurationProvider).ToListAsync();

        return query.ToString();
    }
}