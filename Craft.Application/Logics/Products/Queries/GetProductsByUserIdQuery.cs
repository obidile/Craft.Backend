using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries;

public class GetProductsByUserIdQuery : IRequest<List<Product>>
{
    public string UserId { get; set; }
}

public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, List<Product>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductsByUserIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<Product>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.Include(p => p.Business).Where(p => p.Business.UserId.ToString() == request.UserId).ToListAsync(cancellationToken);

        return products;
    }
}