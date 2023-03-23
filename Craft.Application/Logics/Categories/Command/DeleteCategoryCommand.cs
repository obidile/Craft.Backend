using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;

namespace Craft.Application.Logics.Categories.Command;

public class DeleteCategoryCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public DeleteCategoryCommandHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id);

        if (category == null)
        {
            return "Category not found.";
        }

        if (category.Products.Count > 0)
        {
            return "Cannot delete category as it has products.";
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();

        return "Category deleted successfully.";
    }
}