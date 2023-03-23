using Craft.Application.Common.Interface;
using MediatR;

namespace Craft.Application.Logics.Countries.Command;

public class DeleteCountryCommand : IRequest<string>
{
    public long Id { get; set; }
}
public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, string>
{
    private readonly IApplicationContext _dbContext;
    public DeleteCountryCommandHandler(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _dbContext.Countries.FindAsync(request.Id);
        if (country == null)
        {
            return "Country not found";
        }

        _dbContext.Countries.Remove(country);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return "Country was deleted Successfully";
    }
}