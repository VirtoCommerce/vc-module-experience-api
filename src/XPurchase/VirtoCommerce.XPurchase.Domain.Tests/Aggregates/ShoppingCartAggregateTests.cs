using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Aggregates;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Cart.ValidationErrors;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Customer;
using VirtoCommerce.XPurchase.Models.Marketing.Services;
using VirtoCommerce.XPurchase.Models.Quote;
using Xunit;

using DynamicProperty = VirtoCommerce.XPurchase.Models.DynamicProperty;

namespace VirtoCommerce.XPurchase.Domain.Tests.Aggregates
{
    public class ShoppingCartAggregateTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IProductsRepository> _catalogServiceMock = new Mock<IProductsRepository>();
        private readonly Mock<IPaymentMethodsSearchService> _paymentMethodsSearchServiceMock = new Mock<IPaymentMethodsSearchService>();
        private readonly Mock<IPromotionEvaluator> _promotionEvaluatorMock = new Mock<IPromotionEvaluator>();
        private readonly Mock<IShippingMethodsSearchService> _shippingMethodsSearchServiceMock = new Mock<IShippingMethodsSearchService>();
        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock = new Mock<IShoppingCartService>();
        private readonly Mock<ITaxEvaluator> _taxEvaluatorMock = new Mock<ITaxEvaluator>();

        private readonly ShoppingCartAggregate aggregate;

        public ShoppingCartAggregateTests()
        {
            _fixture.Register(() => new Language("en-US"));
            _fixture.Register(() => new Currency(_fixture.Create<Language>(), "USD"));
            _fixture.Register<IMutablePagedList<DynamicProperty>>(() => null);
            _fixture.Register(() => _fixture.Build<DynamicPropertyName>().With(x => x.Locale, "en-US").Create());
            _fixture.Register(() => _fixture.Build<DynamicPropertyObjectValue>().With(x => x.Locale, "en-US").Create());
            _fixture.Register<IMutablePagedList<SettingEntry>>(() => null);
            _fixture.Register<IMutablePagedList<Contact>>(() => null);
            _fixture.Register<IMutablePagedList<QuoteRequest>>(() => null);
            _fixture.Register<IMutablePagedList<Category>>(() => null);
            _fixture.Register<IMutablePagedList<Product>>(() => null);
            _fixture.Register<IList<Product>>(() => null);
            _fixture.Register<IList<ValidationError>>(() => null);
            _fixture.Register<IMutablePagedList<CatalogProperty>>(() => null);
            _fixture.Register<IMutablePagedList<ProductAssociation>>(() => null);
            _fixture.Register<IMutablePagedList<EditorialReview>>(() => null);
            _fixture.Register(() => _fixture.Build<LineItem>()
                                            .Without(x => x.DynamicProperties)
                                            .With(x => x.IsReadOnly, false)
                                            .Create());
            _fixture.Register(() => _fixture.Build<ShoppingCart>().Without(x => x.DynamicProperties).Create());
            _fixture.Register<Price>(() => null);

            var context = new ShoppingCartContext
            {
            };

            aggregate = new ShoppingCartAggregate(
                _catalogServiceMock.Object,
                _paymentMethodsSearchServiceMock.Object,
                _promotionEvaluatorMock.Object,
                _shippingMethodsSearchServiceMock.Object,
                _shoppingCartServiceMock.Object,
                _taxEvaluatorMock.Object,
                context);
        }

        [Fact]
        public async Task ClearCart_ShouldRemoveItems()
        {
            // Arrange
            var shoppingCart = _fixture.Create<ShoppingCart>();
            await aggregate.TakeCartAsync(shoppingCart);

            // Act
            var result = await aggregate.ClearAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            aggregate.Cart.Items.Should().BeEmpty();
            aggregate.Cart.ItemsCount.Should().Be(0);
            aggregate.Cart.ItemsQuantity.Should().Be(0);
        }
    }
}
