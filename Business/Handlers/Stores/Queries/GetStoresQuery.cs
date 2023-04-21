
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Entities;

namespace Business.Handlers.Stores.Queries
{

    public class GetStoresQuery : IRequest<IDataResult<IEnumerable<Store>>>
    {
        public class GetStoresQueryHandler : IRequestHandler<GetStoresQuery, IDataResult<IEnumerable<Store>>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;

            public GetStoresQueryHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<Store>>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<Store>>(await _storeRepository.GetListAsync());
            }
        }
    }
}