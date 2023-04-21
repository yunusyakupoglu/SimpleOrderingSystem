
using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Stores.ValidationRules;


namespace Business.Handlers.Stores.Commands
{


    public class UpdateStoreCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int CreatedUserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int LastUpdatedUserId { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }
        public bool isReady { get; set; }

        public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, IResult>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;

            public UpdateStoreCommandHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateStoreValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
            {
                var isThereStoreRecord = await _storeRepository.GetAsync(u => u.Id == request.Id);


                isThereStoreRecord.CreatedUserId = request.CreatedUserId;
                isThereStoreRecord.CreatedDate = request.CreatedDate;
                isThereStoreRecord.LastUpdatedUserId = request.LastUpdatedUserId;
                isThereStoreRecord.LastUpdatedDate = System.DateTime.Now;
                isThereStoreRecord.Status = request.Status;
                isThereStoreRecord.isDeleted = request.isDeleted;
                isThereStoreRecord.ProductId = request.ProductId;
                isThereStoreRecord.Stock = request.Stock;
                isThereStoreRecord.isReady = request.isReady;


                _storeRepository.Update(isThereStoreRecord);
                await _storeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
            }
        }
    }
}

