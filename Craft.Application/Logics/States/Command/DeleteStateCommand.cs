using Craft.Application.Common.Interface;
using MediatR;

namespace Craft.Application.Logics.States.Command;

public class DeleteStateCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, string>
{
    private readonly IApplicationContext _dbContext;
    public DeleteStateCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        var model = await _dbContext.States.FindAsync(request.Id);

        if (model == null)
        {
            return "State was not found";
        }

        _dbContext.States.Remove(model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return "State was successfully deleted";
    }
}