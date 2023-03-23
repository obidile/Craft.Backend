using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries;

public class GetProductByIdQuery : IRequest<Product>
{
    public long Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.Include(p => p.Business).Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return product;
    }
}