
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Stores.ValidationRules;
using Entities;

namespace Business.Handlers.Stores.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateStoreCommand : IRequest<IResult>
    {

        public int CreatedUserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int LastUpdatedUserId { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }
        public bool isReady { get; set; }


        public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, IResult>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;
            public CreateStoreCommandHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateStoreValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
            {
                var isThereStoreRecord = _storeRepository.Query().Any(u => u.ProductId == request.ProductId &&
                u.Stock == request.Stock &&
                u.isReady == request.isReady);

                if (isThereStoreRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var addedStore = new Store
                {
                    CreatedUserId = request.CreatedUserId,
                    CreatedDate = System.DateTime.Now,
                    LastUpdatedUserId = request.LastUpdatedUserId,
                    LastUpdatedDate = System.DateTime.Now,
                    Status = true,
                    isDeleted = false,
                    ProductId = request.ProductId,
                    Stock = request.Stock,
                    isReady = request.isReady,
                };

                _storeRepository.Add(addedStore);
                await _storeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Added);
            }
        }
    }
}