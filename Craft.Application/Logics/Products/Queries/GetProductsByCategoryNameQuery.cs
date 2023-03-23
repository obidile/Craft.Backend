using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using Craft.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries;

public class GetProductsByCategoryNameQuery : IRequest<List<ProductModel>>
{
    public string CategoryName { get; set; }
}

public class GetProductsByCategoryNameQueryHandler : IRequestHandler<GetProductsByCategoryNameQuery, List<ProductModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductsByCategoryNameQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ProductModel>> Handle(GetProductsByCategoryNameQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.Where(p => p.Category.Name.ToLower() == request.CategoryName.ToLower()).Include(p => p.Business).Include(p => p.Category)
              .ToListAsync(cancellationToken);

        return _mapper.Map<List<ProductModel>>(products);
    }
}
