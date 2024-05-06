using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using GraphQL;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;
using Xunit;
using Store = VirtoCommerce.StoreModule.Core.Model.Store;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Handlers
{
    public class CreateOrderFromCartCommandHandlerTests : CustomerOrderMockHelper
    {
        [Fact]
        public Task Handle_CartHasValidationErrors_ExceptionThrown()
        {
            // Arrange
            var cart = _fixture.Create<ShoppingCart>();
            var lineItem = _fixture.Create<LineItem>();
            cart.Items = new List<LineItem>() { lineItem };

            var cartAggregate = GetCartAggregateMock(cart);

            var cartService = new Mock<IShoppingCartService>();
            cartService
                .Setup(x => x.GetAsync(It.IsAny<IList<string>>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new[] { cart });
            var aggregationService = new Mock<ICartAggregateRepository>();
            aggregationService
                .Setup(x => x.GetCartForShoppingCartAsync(It.Is<ShoppingCart>(x => x == cart), null))
                .ReturnsAsync(() =>
                {
                    var error = CartErrorDescriber.ProductPriceChangedError(lineItem, lineItem.SalePrice, lineItem.SalePriceWithTax, 0, 0);
                    cartAggregate.ValidationWarnings.Add(error);

                    return cartAggregate;
                });

            var validationContextMock = new Mock<ICartValidationContextFactory>();
            validationContextMock.Setup(x => x.CreateValidationContextAsync(It.IsAny<CartAggregate>()))
                .ReturnsAsync(new CartValidationContext());

            var orderAggregateRepositoryMock = new Mock<ICustomerOrderAggregateRepository>();

            var request = new CreateOrderFromCartCommand(cart.Id);

            var handler = new CreateOrderFromCartCommandHandler(
                cartService.Object,
                orderAggregateRepositoryMock.Object,
                aggregationService.Object,
                validationContextMock.Object);

            // Assert
            return Assert.ThrowsAsync<ExecutionError>(() => handler.Handle(request, CancellationToken.None));
        }

        private static CartAggregate GetCartAggregateMock(ShoppingCart cart)
        {
            var cartAggregate = new CartAggregate(
                Mock.Of<IMarketingPromoEvaluator>(),
                Mock.Of<IShoppingCartTotalsCalculator>(),
                new Mock<ITaxProviderSearchService>().Object,
                Mock.Of<ICartProductService>(),
                Mock.Of<IDynamicPropertyUpdaterService>(),
                Mock.Of<IMapper>(),
                Mock.Of<IMemberOrdersService>(),
                Mock.Of<IMemberService>());

            var contact = new Contact()
            {
                Id = Guid.NewGuid().ToString(),
            };

            cartAggregate.GrabCart(cart, new Store(), contact, new CoreModule.Core.Currency.Currency());

            return cartAggregate;
        }
    }
}
