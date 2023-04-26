using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Orders.Queries
{
    public class GetOrdersDtoQuery : IRequest<IDataResult<IEnumerable<OrderDto>>>
    {
        public class GetOrdersDtoQueryHandler : IRequestHandler<GetOrdersDtoQuery, IDataResult<IEnumerable<OrderDto>>>
        {
            private readonly IOrderRepository _orderRepository;

            public GetOrdersDtoQueryHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<IDataResult<IEnumerable<OrderDto>>> Handle(GetOrdersDtoQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<OrderDto>>(await _orderRepository.GetOrderDto());
            }
        }
    }
}
