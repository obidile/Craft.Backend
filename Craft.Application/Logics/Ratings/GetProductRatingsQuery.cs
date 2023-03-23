using AutoMapper;
using AutoMapper.QueryableExtensions;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.Ratings;

public class GetProductRatingsQuery : IRequest<List<RatingModel>>
{
    public long ProductId { get; set; }
}
public class GetProductRatingsQueryHandler : IRequestHandler<GetProductRatingsQuery, List<RatingModel>>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetProductRatingsQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    async Task<List<RatingModel>> IRequestHandler<GetProductRatingsQuery, List<RatingModel>>.Handle(GetProductRatingsQuery request, CancellationToken cancellationToken)
    {
        var productRatings = await _dbContext.Ratings.Include(x => x.User).Where(x => x.Product.Id == request.ProductId).ProjectTo<RatingModel>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

        // instance for shuffling
        var random = new Random();

        // Shuffle the list by generating a random number for each item's order
        var shuffledRatings = productRatings.OrderBy(x => random.Next()).ThenBy(x => x.CreatedDate).ToList();

        return shuffledRatings;
    }
}
