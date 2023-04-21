
using Business.Handlers.Stores.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Stores.Queries.GetStoreQuery;
using Entities.Concrete;
using static Business.Handlers.Stores.Queries.GetStoresQuery;
using static Business.Handlers.Stores.Commands.CreateStoreCommand;
using Business.Handlers.Stores.Commands;
using Business.Constants;
using static Business.Handlers.Stores.Commands.UpdateStoreCommand;
using static Business.Handlers.Stores.Commands.DeleteStoreCommand;
using MediatR;
using System.Linq;
using FluentAssertions;
using Entities;

namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class StoreHandlerTests
    {
        Mock<IStoreRepository> _storeRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _storeRepository = new Mock<IStoreRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Store_GetQuery_Success()
        {
            //Arrange
            var query = new GetStoreQuery();

            _storeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Store, bool>>>())).ReturnsAsync(new Store()
//propertyler buraya yazılacak
//{																		
//StoreId = 1,
//StoreName = "Test"
//}
);

            var handler = new GetStoreQueryHandler(_storeRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.StoreId.Should().Be(1);

        }

        [Test]
        public async Task Store_GetQueries_Success()
        {
            //Arrange
            var query = new GetStoresQuery();

            _storeRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Store, bool>>>()))
                        .ReturnsAsync(new List<Store> { new Store() { /*TODO:propertyler buraya yazılacak StoreId = 1, StoreName = "test"*/ } });

            var handler = new GetStoresQueryHandler(_storeRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Store>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Store_CreateCommand_Success()
        {
            Store rt = null;
            //Arrange
            var command = new CreateStoreCommand();
            //propertyler buraya yazılacak
            //command.StoreName = "deneme";

            _storeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Store, bool>>>()))
                        .ReturnsAsync(rt);

            _storeRepository.Setup(x => x.Add(It.IsAny<Store>())).Returns(new Store());

            var handler = new CreateStoreCommandHandler(_storeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _storeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Store_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateStoreCommand();
            //propertyler buraya yazılacak 
            //command.StoreName = "test";

            _storeRepository.Setup(x => x.Query())
                                           .Returns(new List<Store> { new Store() { /*TODO:propertyler buraya yazılacak StoreId = 1, StoreName = "test"*/ } }.AsQueryable());

            _storeRepository.Setup(x => x.Add(It.IsAny<Store>())).Returns(new Store());

            var handler = new CreateStoreCommandHandler(_storeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Store_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateStoreCommand();
            //command.StoreName = "test";

            _storeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Store, bool>>>()))
                        .ReturnsAsync(new Store() { /*TODO:propertyler buraya yazılacak StoreId = 1, StoreName = "deneme"*/ });

            _storeRepository.Setup(x => x.Update(It.IsAny<Store>())).Returns(new Store());

            var handler = new UpdateStoreCommandHandler(_storeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _storeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Store_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteStoreCommand();

            _storeRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Store, bool>>>()))
                        .ReturnsAsync(new Store() { /*TODO:propertyler buraya yazılacak StoreId = 1, StoreName = "deneme"*/});

            _storeRepository.Setup(x => x.Delete(It.IsAny<Store>()));

            var handler = new DeleteStoreCommandHandler(_storeRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _storeRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

