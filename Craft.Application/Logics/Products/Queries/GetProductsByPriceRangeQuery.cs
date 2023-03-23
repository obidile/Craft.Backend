using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries;

public class GetProductsByPriceRangeQuery : IRequest<List<ProductModel>>
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}

public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, List<ProductModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductsByPriceRangeQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ProductModel>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.Where(p => p.Price >= request.MinPrice && p.Price <= request.MaxPrice)
            .Include(p => p.Business)
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ProductModel>>(products);
    }
}