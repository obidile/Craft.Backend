using Craft.Application.Common.Interface;
using MediatR;

namespace Craft.Application.Logics.Schools.Command;

public class DeleteSchoolCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, string>
{
    private readonly IApplicationContext _dbContext;
    public DeleteSchoolCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<string> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var model = await _dbContext.Schools.FindAsync(request.Id);

        if (model == null)
        {
            return "School was not found";
        }

        _dbContext.Schools.Remove(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return "School was successfully deleted";
    }
}