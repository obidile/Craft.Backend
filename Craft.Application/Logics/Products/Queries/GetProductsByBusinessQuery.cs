using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries;

public class GetProductsByBusinessQuery : IRequest<List<ProductModel>>
{
    public string BusinessName { get; set; }
}

public class GetProductsByBusinessQueryHandler : IRequestHandler<GetProductsByBusinessQuery, List<ProductModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductsByBusinessQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ProductModel>> Handle(GetProductsByBusinessQuery request, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.Where(p => p.Name.ToLower() == request.BusinessName.ToLower()).Include(p => p.Business)
            .Include(p => p.Category).ToListAsync(cancellationToken);

        return _mapper.Map<List<ProductModel>>(products);
    }
}
