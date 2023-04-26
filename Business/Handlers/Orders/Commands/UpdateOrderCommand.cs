
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
using Business.Handlers.Orders.ValidationRules;
using Business.Handlers.Stores.Commands;
using Business.Handlers.Stores.Queries;
using System;

namespace Business.Handlers.Orders.Commands
{


    public class UpdateOrderCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public int CreatedUserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int LastUpdatedUserId { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, IResult>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMediator _mediator;

            public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateOrderValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
            {
                var isThereOrderRecord = await _orderRepository.GetAsync(u => u.Id == request.Id);
                var getStoreByProductIdAndSize = _mediator.Send(new GetStoreByProductIdAndSizeQuery { ProductId = request.ProductId });
                var newStoreStock = isThereOrderRecord.Stock - request.Stock;

                isThereOrderRecord.CreatedUserId = request.CreatedUserId;
                isThereOrderRecord.CreatedDate = request.CreatedDate;
                isThereOrderRecord.LastUpdatedUserId = request.LastUpdatedUserId;
                isThereOrderRecord.LastUpdatedDate = System.DateTime.Now;
                isThereOrderRecord.Status = request.Status;
                isThereOrderRecord.isDeleted = request.isDeleted;
                isThereOrderRecord.CustomerId = request.CustomerId;
                isThereOrderRecord.ProductId = request.ProductId;
                isThereOrderRecord.Stock = request.Stock;


                var update = await _mediator.Send(new UpdateStoreCommand
                {
                    isReady = getStoreByProductIdAndSize.Result.Data.IsReady,
                    CreatedDate = getStoreByProductIdAndSize.Result.Data.CreatedDate,
                    CreatedUserId = getStoreByProductIdAndSize.Result.Data.CreatedUserId,
                    isDeleted = getStoreByProductIdAndSize.Result.Data.isDeleted,
                    LastUpdatedDate = DateTime.Now,
                    LastUpdatedUserId = request.LastUpdatedUserId,
                    Status = getStoreByProductIdAndSize.Result.Data.Status,
                    Id = getStoreByProductIdAndSize.Result.Data.Id,
                    Stock = newStoreStock + getStoreByProductIdAndSize.Result.Data.Stock,
                    ProductId = getStoreByProductIdAndSize.Result.Data.ProductId
                });

                if (update.Success)
                {
                _orderRepository.Update(isThereOrderRecord);
                await _orderRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Updated);
                }
                return new ErrorResult("Sipariş kaydı güncellenemedi.");
            }
        }
    }
}

