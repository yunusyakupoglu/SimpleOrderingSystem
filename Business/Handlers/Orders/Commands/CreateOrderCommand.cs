
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
using Business.Handlers.Orders.ValidationRules;
using Entities;
using Business.Handlers.Orders.Queries;
using Business.Handlers.Stores.Queries;
using Business.Handlers.Stores.Commands;
using System;

namespace Business.Handlers.Orders.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateOrderCommand : IRequest<IResult>
    {

        public int CreatedUserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int LastUpdatedUserId { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }


        public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, IResult>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMediator _mediator;
            public CreateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateOrderValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            {
                var isThereOrderRecord = _orderRepository.Query().Any(u => u.CreatedUserId == request.CreatedUserId);

                if (isThereOrderRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);


                var isExistStore = _mediator.Send(new IsExistStoreQuery { ProductId = request.ProductId, Stock = request.Stock });

                if (isExistStore.Result.Data)
                {
                    var getStoreByProductIdAndSize = _mediator.Send(new GetStoreByProductIdAndSizeQuery { ProductId = request.ProductId });
                    var addedOrder = new Order
                    {
                        CreatedUserId = request.CreatedUserId,
                        CreatedDate = System.DateTime.Now,
                        LastUpdatedUserId = request.LastUpdatedUserId,
                        LastUpdatedDate = System.DateTime.Now,
                        Status = true,
                        isDeleted = false,
                        CustomerId = request.CustomerId,
                        ProductId = request.ProductId,
                        Stock = request.Stock,

                    };



                    _orderRepository.Add(addedOrder);
                    await _orderRepository.SaveChangesAsync();
                    await _mediator.Send(new UpdateStoreCommand
                    {
                        isReady = getStoreByProductIdAndSize.Result.Data.IsReady,
                        CreatedDate = getStoreByProductIdAndSize.Result.Data.CreatedDateByStore,
                        CreatedUserId = getStoreByProductIdAndSize.Result.Data.CreatedUserIdByStore,
                        isDeleted = getStoreByProductIdAndSize.Result.Data.isDeletedByStore,
                        LastUpdatedDate = DateTime.Now,
                        LastUpdatedUserId = request.LastUpdatedUserId,
                        Status = getStoreByProductIdAndSize.Result.Data.StatusByStore,
                        Id = getStoreByProductIdAndSize.Result.Data.StoreId,
                        Stock = getStoreByProductIdAndSize.Result.Data.Stock - request.Stock,
                        ProductId = getStoreByProductIdAndSize.Result.Data.ProductId
                    });
                    return new SuccessResult(Messages.Added);
                }
                return new ErrorResult("Sipariş Kaydı oluşturulamadı.");
            }
        }
    }
}