using Craft.Application.Common.Interface;
using MediatR;

namespace Craft.Application.Logics.Users.Command;

public class DeleteUserCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, string>
{
    private readonly IApplicationContext _dbContext;

    public DeleteUserCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(request.Id);
        if (user == null)
        {
            return "User was not found";
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return "ArtWork was deleted Successfully";
    }
}