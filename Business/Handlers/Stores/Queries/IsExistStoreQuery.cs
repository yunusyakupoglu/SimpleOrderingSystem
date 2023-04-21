using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Stores.Queries
{
    public class IsExistStoreQuery : IRequest<IDataResult<bool>>
    {
        public int ProductId { get; set; }
        //public int SizeId { get; set; }
        public int Stock { get; set; }

        public class IsExistStoreQueryHandler : IRequestHandler<IsExistStoreQuery, IDataResult<bool>>
        {
            private readonly IStoreRepository _storeRepository;

            public IsExistStoreQueryHandler(IStoreRepository storeRepository)
            {
                _storeRepository = storeRepository;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<bool>> Handle(IsExistStoreQuery request, CancellationToken cancellationToken)
            {
                var store = await _storeRepository.GetAsync(p => p.ProductId == request.ProductId);
                bool result = store!=null && store.Stock >= request.Stock;
                return new SuccessDataResult<bool>(result);
            }
        }
    }
}
