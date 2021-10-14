using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XPurchase.Commands;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class AddCartAddressCommandHandlerTests : XPurchaseMoqHelper
    {
        [Fact]
        public async Task Handle_RequestWithCartId_AddCartAddressAsyncCalled()
        {
            // Arragne
            var addressType = (int)CoreModule.Core.Common.AddressType.Billing;

            var cartAggregate = GetValidCartAggregate();
            cartAggregate.Cart.Addresses.Clear();

            var cartAggregateRepositoryMock = new Mock<ICartAggregateRepository>();
            cartAggregateRepositoryMock.Setup(x => x.GetCartByIdAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(cartAggregate);

            var handler = new AddCartAddressCommandHandler(cartAggregateRepositoryMock.Object);
            var request = new AddCartAddressCommand()
            {
                Address = new ExpCartAddress()
                {
                    AddressType = new Optional<int>(addressType),
                },
                CartId = Guid.NewGuid().ToString()
            };

            // Act
            var aggregate = await handler.Handle(request, CancellationToken.None);

            // Assert
            cartAggregate.Cart.Addresses.Should().Contain(x => (int)x.AddressType == addressType);
            cartAggregateRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<CartAggregate>()), Times.Once);
        }
    }
}
