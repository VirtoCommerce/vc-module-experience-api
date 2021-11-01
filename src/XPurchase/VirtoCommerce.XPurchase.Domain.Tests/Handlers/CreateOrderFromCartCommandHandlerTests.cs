using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.ExperienceApiModule.XOrder;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Handlers
{
    public class CreateOrderFromCartCommandHandlerTests : XPurchaseMoqHelper
    {

        /// <summary>
        /// To test if soft cart delete is calling in the create order routine
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Handle_CreateOrder_EnsureCartDeleted()
        {
            // Arrange
            var cartService = new Mock<ShoppingCartService>(null, null, null, null);
            var deleteCalled = false;
            cartService.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ShoppingCart());
            cartService.Setup(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Returns(() =>
                        {
                            deleteCalled = true;
                            return Task.CompletedTask;
                        });

            var customerAggrRep = new Mock<ICustomerOrderAggregateRepository>();
            customerAggrRep.Setup(x => x.CreateOrderFromCart(It.IsAny<ShoppingCart>()))
                .ReturnsAsync(new CustomerOrderAggregate(null, null));

            // Take action
            var handler = new CreateOrderFromCartCommandHandler(cartService.Object, customerAggrRep.Object);
            await handler.Handle(new CreateOrderFromCartCommand(""), CancellationToken.None);

            // Assert
            deleteCalled.Should().BeTrue();
        }
    }
}
