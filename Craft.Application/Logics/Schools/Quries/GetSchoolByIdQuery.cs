using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Schools.Quries;

public class GetSchoolByIdQuery : IRequest<string>
{
    public GetSchoolByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetSchoolByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<string> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var data = await _dbContext.Schools.AsNoTracking().Where(x => x.Id == request.Id).Include(x => x.State).ProjectTo<SchoolModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

        if (data == null)
        {
            return "No School with the specified ID was found.";
        }

        return data.ToString();
    }
}