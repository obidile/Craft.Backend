using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics;

public class GetBusinessRatingsQuery : IRequest<List<RatingModel>>
{
    public long BusinessId { get; set; }
}
public class GetBusinessRatingsQueryHandler : IRequestHandler<GetBusinessRatingsQuery, List<RatingModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetBusinessRatingsQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    async Task<List<RatingModel>> IRequestHandler<GetBusinessRatingsQuery, List<RatingModel>>.Handle(GetBusinessRatingsQuery request, CancellationToken cancellationToken)
    {
        var businessRatings = await _dbContext.Ratings
            .Include(x => x.User).Where(x => x.Business.Id == request.BusinessId).ProjectTo<RatingModel>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // instance for shuffling
        var random = new Random();

        // Shuffle the list by generating a random number for each item's order
        var shuffledRatings = businessRatings.OrderBy(x => random.Next()).ThenBy(x => x.CreatedDate).ToList();

        return shuffledRatings;
    }
}