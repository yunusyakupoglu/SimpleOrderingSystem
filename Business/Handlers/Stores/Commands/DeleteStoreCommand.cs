
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Business.Handlers.Stores.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteStoreCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, IResult>
        {
            private readonly IStoreRepository _storeRepository;
            private readonly IMediator _mediator;

            public DeleteStoreCommandHandler(IStoreRepository storeRepository, IMediator mediator)
            {
                _storeRepository = storeRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
            {
                var storeToDelete = _storeRepository.Get(p => p.Id == request.Id);
                storeToDelete.isDeleted = true;
                _storeRepository.Update(storeToDelete);
                await _storeRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

