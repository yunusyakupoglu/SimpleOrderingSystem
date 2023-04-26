using Amazon.Runtime.Internal;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
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
    public class GetOrderDtoQuery : IRequest<IDataResult<OrderDto>>
    {
        public int Id { get; set; }

        public class GetOrderDtoQueryHandler : IRequestHandler<GetOrderDtoQuery, IDataResult<OrderDto>>
        {
            private readonly IOrderRepository _orderRepository;

            public GetOrderDtoQueryHandler(IOrderRepository orderRepository)
            {
                _orderRepository = orderRepository;
            }

            public async Task<IDataResult<OrderDto>> Handle(GetOrderDtoQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<OrderDto>(await _orderRepository.GetDtoAsync(request.Id));
            }
        }
    }
}
