using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities;
using Entities.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Stores.Queries
{

    public class getStoreByProductIdQuery : IRequest<IDataResult<Store>>
    {
        public int ProductId { get; set; }

        public class getStoreByProductIdQueryHandler : IRequestHandler<getStoreByProductIdQuery, IDataResult<Store>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IProductRepository _productRepository;

            public getStoreByProductIdQueryHandler(IStoreRepository storeRepository, IProductRepository productRepository)
            {
                _storeRepository = storeRepository;
                _productRepository = productRepository;
            }
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Store>> Handle(getStoreByProductIdQuery request, CancellationToken cancellationToken)
            {
                var data = await _storeRepository.GetStoreByProductId(request.ProductId);
                return new SuccessDataResult<Store>(data);
            }
        }
    }
}
