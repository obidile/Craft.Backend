using Craft.Application.Common.Interface;
using Craft.Domain.Entities;
using MediatR;

namespace Craft.Application.Logics.Businesses.Command;

public class DeleteBusinessCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteBusinessCommandHandler : IRequestHandler<DeleteBusinessCommand, string>
{
    private readonly IApplicationContext _dbContext;
    public DeleteBusinessCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<string> Handle(DeleteBusinessCommand request, CancellationToken cancellationToken)
    {
        //var user = await _dbContext.Users.FindAsync(request.Id);
        var business = await _dbContext.Businesses.FindAsync(request.Id);
        if (business == null)
        {
            return "Business was not found";
        }

        _dbContext.Businesses.Remove(business);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return "Business was deleted Successfully";
    }
}