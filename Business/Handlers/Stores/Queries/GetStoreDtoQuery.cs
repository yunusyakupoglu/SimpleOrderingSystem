using Amazon.Runtime.Internal;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities;
using Entities.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Stores.Queries
{
    public class GetStoreDtoQuery : IRequest<IDataResult<ProductWithStoreDto>>
    {
        public int Id { get; set; }

        public class GetStoreDtoQueryHandler : IRequestHandler<GetStoreDtoQuery, IDataResult<ProductWithStoreDto>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;

            public GetStoreDtoQueryHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<ProductWithStoreDto>> Handle(GetStoreDtoQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<ProductWithStoreDto>(await _storeRepository.GetDtoAsync(request.Id));
            }
        }
    }
}
