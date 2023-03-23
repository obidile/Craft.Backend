using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Products.Queries
{
    public class GetAllProductsQuery : IRequest<List<ProductModel>>
    {
        public string SearchTerm { get; set; }
    }

    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductModel>>
    {
        private readonly IApplicationContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IApplicationContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<ProductModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Products.Include(p => p.Business).Include(p => p.Category).AsQueryable();

            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return _mapper.Map<List<ProductModel>>(await query.ToListAsync(cancellationToken));
            }

            var searchTerm = request.SearchTerm.ToLower();
            var products = await query.Where(p => p.Name.ToLower().Contains(searchTerm) || p.Description.ToLower().Contains(searchTerm) ||
                p.Category.Name.ToLower().Contains(searchTerm)).ToListAsync(cancellationToken);


            return _mapper.Map<List<ProductModel>>(products);
        }
    }
}
