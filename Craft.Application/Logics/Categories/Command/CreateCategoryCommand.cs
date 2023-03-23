using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;

namespace Craft.Application.Logics.Categories.Command;

public class CreateCategoryCommand : IRequest<string>
{
    public string Name { get; set; }
}
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"{category.Name} Category has been successfully created";
    }
}