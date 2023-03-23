using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;

namespace Craft.Application.Logics.Categories.Command;

public class UpdateCategoryCommand : IRequest<string>
{
    public long CategoryId { get; set; }
    public string Name { get; set; }
}
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public UpdateCategoryCommandHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.CategoryId);

        if (category == null)
        {
            return "Category not found";
        }

        category.Name = request.Name;
        await _dbContext.SaveChangesAsync();

        return "Category updated successfully";
    }
}