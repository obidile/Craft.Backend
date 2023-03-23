using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Categories.Queries;

public class GetCategoryById : IRequest<string>
{
    public long Id { get; set; }
}
public class GetCategoryByIdHandler : IRequestHandler<GetCategoryById, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetCategoryByIdHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetCategoryById request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Categories.AsNoTracking().Where(x => x.Id == request.Id).ProjectTo<CategoryModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

        if (data == null)
        {
            throw new Exception("No Category with the specified ID was found.");
        }

        return data.ToString();
    }
}