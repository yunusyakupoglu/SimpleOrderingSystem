using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Stores.Queries
{
    public class ProductWithStore
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedDateByStore { get; set; }
        public int CreatedUserIdByStore { get; set; }
        public bool isDeletedByStore { get; set; }
        public bool StatusByStore { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }
        public bool IsReady { get; set; }
    }

    public class GetStoreByProductIdAndSizeQuery : IRequest<IDataResult<ProductWithStore>>
    {
        public int ProductId { get; set; }
        public string Size { get; set; }

        public class GetStoreByProductIdAndSizeQueryHandler : IRequestHandler<GetStoreByProductIdAndSizeQuery, IDataResult<ProductWithStore>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IProductRepository _productRepository;

            public GetStoreByProductIdAndSizeQueryHandler(IStoreRepository storeRepository, IProductRepository productRepository)
            {
                _storeRepository = storeRepository;
                _productRepository = productRepository;
            }

            public async Task<IDataResult<ProductWithStore>> Handle(GetStoreByProductIdAndSizeQuery request, CancellationToken cancellationToken)
            {
                var store = await _storeRepository.GetListAsync(s => s.ProductId == request.ProductId);
                var product = await _productRepository.GetListAsync(p => p.Id == request.ProductId);

                var result = (from p in product 
                              join s in store 
                              on p.Id equals s.ProductId
                              select new ProductWithStore
                              {
                                  ProductId = p.Id,
                                  Name = p.Name,
                                  Color = p.Color,
                                  Size = p.Size,
                                  Stock = s.Stock,
                                  IsReady = s.isReady,
                                  StoreId = s.Id,
                                  CreatedDateByStore = s.CreatedDate,
                                  CreatedUserIdByStore = s.CreatedUserId,
                                  isDeletedByStore = s.isDeleted,
                                  StatusByStore = s.Status
                              }).Single();

                return new SuccessDataResult<ProductWithStore>(result);
            }
        }
    }
}
