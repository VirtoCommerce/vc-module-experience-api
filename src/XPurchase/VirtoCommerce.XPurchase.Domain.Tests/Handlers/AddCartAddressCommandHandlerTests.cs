using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.XPurchase.Commands;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class AddCartAddressCommandHandlerTests
    {
        [Fact]
        public async Task Handle_RequestWithCartId_AddCartAddressAsyncCalled()
        {
            // Arragne
            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            var cartAggregateMock = new Mock<CartAggregate>(MockBehavior.Loose, null, null, null, null, null, null);
            cartAggregateRepositoryMock.Setup(x => x.GetCartByIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(cartAggregateMock.Object);

            var handler = new AddCartAddressCommandHandler(cartAggregateRepositoryMock.Object);
            var request = new AddCartAddressCommand() { CartId = Guid.NewGuid().ToString() };

            // Act
            var aggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            cartAggregateMock.Verify(x => x.AddOrUpdateCartAddressByTypeAsync(It.IsAny<Address>()), Times.Once);
            cartAggregateRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<CartAggregate>()), Times.Once);
        }
    }
}
