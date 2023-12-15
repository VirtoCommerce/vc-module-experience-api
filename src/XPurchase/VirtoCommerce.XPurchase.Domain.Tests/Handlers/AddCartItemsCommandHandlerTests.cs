using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.XPurchase.Commands;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class AddCartItemsCommandHandlerTests
    {
        [Fact]
        public async Task Handle_RequestWithCartId_AddItemsAsyncCalled()
        {
            // Arragne
            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            var cartAggregateMock = new Mock<CartAggregate>(MockBehavior.Loose, null, null, null, null, null, null, null, null);
            cartAggregateRepositoryMock.Setup(x => x.GetCartByIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(cartAggregateMock.Object);

            var handler = new AddCartItemsCommandHandler(cartAggregateRepositoryMock.Object);
            var request = new AddCartItemsCommand() { CartId = Guid.NewGuid().ToString() };

            // Act
            var aggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            cartAggregateMock.Verify(x => x.AddItemsAsync(It.IsAny<ICollection<NewCartItem>>()), Times.Once);
            cartAggregateRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<CartAggregate>()), Times.Once);
        }
    }
}
