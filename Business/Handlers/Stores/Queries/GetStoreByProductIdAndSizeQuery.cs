using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Stores.Queries
{

    public class GetStoreByProductIdAndSizeQuery : IRequest<IDataResult<ProductWithStoreDto>>
    {
        public int ProductId { get; set; }
        public string Size { get; set; }

        public class GetStoreByProductIdAndSizeQueryHandler : IRequestHandler<GetStoreByProductIdAndSizeQuery, IDataResult<ProductWithStoreDto>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IProductRepository _productRepository;

            public GetStoreByProductIdAndSizeQueryHandler(IStoreRepository storeRepository, IProductRepository productRepository)
            {
                _storeRepository = storeRepository;
                _productRepository = productRepository;
            }

            public async Task<IDataResult<ProductWithStoreDto>> Handle(GetStoreByProductIdAndSizeQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<ProductWithStoreDto>(await _storeRepository.GetProductWithStore(request.ProductId));
            }
        }
    }
}
