using AutoMapper;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Craft.Application.Logics.OrderItems.Quries;

public class GetOrderItemByIdQuery : IRequest<string>
{
    public GetOrderItemByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}
public class GetOrderItemByIdQueryHandler : IRequestHandler<GetOrderItemByIdQuery, string>
{
    private readonly IApplicationContext _dbContext;
    private readonly IMapper _mapper;
    public GetOrderItemByIdQueryHandler(IApplicationContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<string> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
    {
        var orderItem = await _dbContext.OrderItems.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == request.Id);

        if (orderItem == null)
        {
            return "Item Id not found";
        }

        var orderItemModel = _mapper.Map<OrderItemModel>(orderItem);

        return orderItemModel.ToString();
    }
}