using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Users.Quries;

public class GetUserByIdQuery : IRequest<UserModel>
{
    public GetUserByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserModel>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<UserModel> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Users.AsNoTracking().Where(x => x.Id == request.Id).ProjectTo<UserModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

        if (data == null)
        {
            throw new Exception("No User with the specified ID was found.");
        }
        return data;

    }
}
