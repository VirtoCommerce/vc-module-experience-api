using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Data.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.XOrder;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
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
            var cart = new ShoppingCart()
            {
                Name = "default",
                Currency = "USD",
                CustomerId = Guid.NewGuid().ToString(),
            };

            var cartService = new Mock<ShoppingCartService>(null, null, null, null);
            var deleteCalled = false;
            cartService.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cart);
            cartService.Setup(x => x.DeleteAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Returns(() =>
                        {
                            deleteCalled = true;
                            return Task.CompletedTask;
                        });

            var customerAggrRep = new Mock<ICustomerOrderAggregateRepository>();
            customerAggrRep.Setup(x => x.CreateOrderFromCart(It.IsAny<ShoppingCart>()))
                .ReturnsAsync(new CustomerOrderAggregate(null, null));

            var cartAggr = new CartAggregate(null, null, null, null, null, null, null, null);
            cartAggr.GrabCart(cart, new Store(), new Contact(), new Currency());
            var cartAggrRep = new Mock<ICartAggregateRepository>();
            cartAggrRep.Setup(x => x.GetCartForShoppingCartAsync(It.IsAny<ShoppingCart>(), null))
                .ReturnsAsync(cartAggr);

            var contextFactory = new Mock<ICartValidationContextFactory>();
            contextFactory.Setup(x => x.CreateValidationContextAsync(It.IsAny<CartAggregate>()))
                .ReturnsAsync(new CartValidationContext());

            // Take action
            var handler = new CreateOrderFromCartCommandHandler(cartService.Object, customerAggrRep.Object, cartAggrRep.Object, contextFactory.Object);
            await handler.Handle(new CreateOrderFromCartCommand(""), CancellationToken.None);

            // Assert
            deleteCalled.Should().BeTrue();
        }
    }
}
