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
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XOrder.Commands;
using VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;
using Xunit;
using Cart = VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Handlers
{
    public class CreateOrderFromCartCommandHandlerTests : CustomerOrderMockHelper
    {
        [Fact]
        public async Task Handle_CartHasValidationErrors_ExeptionThrown()
        {
            // Arrange
            var cart = _fixture.Create<Cart.ShoppingCart>();
            var lineItem = _fixture.Create<Cart.LineItem>();
            cart.Items = new List<LineItem>() { lineItem };

            var cartAggregate = GetCartAggregateMock(cart);

            var cartService = new ShoppingCartServiceStub(cart);
            var aggregationSerive = new Mock<ICartAggregateRepository>();
            aggregationSerive
                .Setup(x => x.GetCartForShoppingCartAsync(It.Is<Cart.ShoppingCart>(x => x == cart), null))
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

            var handler = new CreateOrderFromCartCommandHandler(cartService,
                orderAggregateRepositoryMock.Object,
                aggregationSerive.Object,
                validationContextMock.Object);

            // Assert
            await Assert.ThrowsAsync<ExecutionError>(() => handler.Handle(request, CancellationToken.None));
        }

        private static CartAggregate GetCartAggregateMock(ShoppingCart cart)
        {
            var cartAggregate = new CartAggregate(
                Mock.Of<IMarketingPromoEvaluator>(),
                Mock.Of<IShoppingCartTotalsCalculator>(),
                Mock.Of<ITaxProviderSearchService>(),
                Mock.Of<ICartProductService>(),
                Mock.Of<IDynamicPropertyUpdaterService>(),
                Mock.Of<IMapper>(),
                Mock.Of<IMemberOrdersService>());

            var contact = new Contact()
            {
                Id = Guid.NewGuid().ToString(),
            };

            cartAggregate.GrabCart(cart, new Store(), contact, new CoreModule.Core.Currency.Currency());

            return cartAggregate;
        }

        public class ShoppingCartServiceStub : ICrudService<ShoppingCart>, IShoppingCartService
        {
            private readonly ShoppingCart _cart;

            public ShoppingCartServiceStub(ShoppingCart cart)
            {
                _cart = cart;
            }

            public Task DeleteAsync(string[] cartIds, bool softDelete = false)
            {
                throw new NotImplementedException();
            }

            public Task DeleteAsync(IEnumerable<string> ids, bool softDelete = false)
            {
                throw new NotImplementedException();
            }

            public Task<IReadOnlyCollection<ShoppingCart>> GetAsync(List<string> ids, string responseGroup = null)
            {
                throw new NotImplementedException();
            }

            public Task<ShoppingCart> GetByIdAsync(string cartId, string responseGroup = null)
            {
                return Task.FromResult(_cart);
            }

            public Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<ShoppingCart>> GetByIdsAsync(IEnumerable<string> ids, string responseGroup = null)
            {
                throw new NotImplementedException();
            }

            public Task SaveChangesAsync(ShoppingCart[] carts)
            {
                throw new NotImplementedException();
            }

            public Task SaveChangesAsync(IEnumerable<ShoppingCart> models)
            {
                throw new NotImplementedException();
            }
        }
    }
}
