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

namespace Business.Handlers.Stores.Queries
{
    public class GetStoreDtosQuery : IRequest<IDataResult<IEnumerable<StoreDto>>>
    {
        public class GetStoreDtosQueryHandler : IRequestHandler<GetStoreDtosQuery, IDataResult<IEnumerable<StoreDto>>>
        {
            private readonly IStoreRepository _storeRepository;

            public GetStoreDtosQueryHandler(IStoreRepository storeRepository)
            {
                _storeRepository = storeRepository;
            }

            public async Task<IDataResult<IEnumerable<StoreDto>>> Handle(GetStoreDtosQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<StoreDto>>(await _storeRepository.GetStoreDto());

            }
        }
    }
}
