using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Categories.Queries;

public class GetCategoriesQuery : IRequest<string>
{
    public string Name { get; set; }
}
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetCategoriesQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Categories.AsNoTracking();
        query = query.Where(x => x.Name.ToLower() == request.Name.ToLower());

        var list = await query.OrderBy(x => x.Name).ToListAsync();

        var result = _mapper.Map<List<CategoryModel>>(list);
        return result.ToString();
    }
}