using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Entities;

namespace Business.Handlers.Stores.Queries
{
    public class GetStoreQuery : IRequest<IDataResult<Store>>
    {
        public int Id { get; set; }

        public class GetStoreQueryHandler : IRequestHandler<GetStoreQuery, IDataResult<Store>>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;

            public GetStoreQueryHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Store>> Handle(GetStoreQuery request, CancellationToken cancellationToken)
            {
                var store = await _storeRepository.GetAsync(p => p.Id == request.Id);
                return new SuccessDataResult<Store>(store);
            }
        }
    }
}
