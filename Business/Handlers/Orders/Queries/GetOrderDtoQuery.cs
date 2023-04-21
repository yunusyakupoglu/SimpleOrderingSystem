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
    public class GetOrderDtoQuery : IRequest<IDataResult<IEnumerable<OrderDto>>>
    {
        public class GetOrderDtoQueryHandler : IRequestHandler<GetOrderDtoQuery, IDataResult<IEnumerable<OrderDto>>>
        {
            private readonly IOrderRepository _orderRepository;

            public GetOrderDtoQueryHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<IDataResult<IEnumerable<OrderDto>>> Handle(GetOrderDtoQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<OrderDto>>(await _orderRepository.GetOrderDto());
            }
        }
    }
}
