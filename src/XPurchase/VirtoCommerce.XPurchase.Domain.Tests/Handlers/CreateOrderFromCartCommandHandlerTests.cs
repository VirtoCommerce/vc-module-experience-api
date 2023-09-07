using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
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
        /// To test if line items are deleted in the create order routine
        /// </summary>
        [Fact]
        public async Task Handle_CreateOrder_EnsureSelectedLineItemsDeleted()
        {
            // Arrange
            var cart = new ShoppingCart()
            {
                Name = "default",
                Currency = "USD",
                CustomerId = Guid.NewGuid().ToString(),
                Items = new List<LineItem>
                {
                    new LineItem()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SelectedForCheckout = true,
                    },
                    new LineItem()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SelectedForCheckout = false,
                    },
                }
            };

            var cartService = new Mock<IShoppingCartService>();
            cartService.Setup(x => x.GetAsync(It.IsAny<IList<string>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new[] { cart });

            var customerAggrRep = new Mock<ICustomerOrderAggregateRepository>();
            customerAggrRep.Setup(x => x.CreateOrderFromCart(It.IsAny<ShoppingCart>()))
                .ReturnsAsync(new CustomerOrderAggregate(null, null));

            var cartAggr = new CartAggregate(null, null, null, null, null, null, null);
            cartAggr.GrabCart(cart, new Store(), new Contact(), new Currency());
            var cartAggrRep = new Mock<ICartAggregateRepository>();
            cartAggrRep.Setup(x => x.GetCartForShoppingCartAsync(It.IsAny<ShoppingCart>(), null))
                .ReturnsAsync(cartAggr);

            var contextFactory = new Mock<ICartValidationContextFactory>();
            contextFactory.Setup(x => x.CreateValidationContextAsync(It.IsAny<CartAggregate>()))
                .ReturnsAsync(new CartValidationContext());

            var handler = new CreateOrderFromCartCommandHandler(cartService.Object, customerAggrRep.Object, cartAggrRep.Object, contextFactory.Object)
            {
                ValidationRuleSet = "default"
            };

            // Act
            await handler.Handle(new CreateOrderFromCartCommand(""), CancellationToken.None);

            // Assert
            cart.Items.Count.Should().Be(1);
        }
    }
}
