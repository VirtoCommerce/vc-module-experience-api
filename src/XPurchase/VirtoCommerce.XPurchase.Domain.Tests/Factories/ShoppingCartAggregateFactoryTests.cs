using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Factories;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Customer;
using VirtoCommerce.XPurchase.Models.Enums;
using VirtoCommerce.XPurchase.Models.Marketing.Services;
using VirtoCommerce.XPurchase.Models.Quote;
using Xunit;
using DynamicProperty = VirtoCommerce.XPurchase.Models.DynamicProperty;

namespace VirtoCommerce.ExpirienceApiModule.XPurchase.Domain.Tests
{
    public class ShoppingCartAggregateFactoryTests
    {
        // Mocks
        private readonly Mock<ICatalogService> _catalogServiceMock = new Mock<ICatalogService>();

        // Fixture
        private readonly Fixture _fixture;

        private readonly Mock<IPaymentMethodsSearchService> _paymentMethodsSearchServiceMock = new Mock<IPaymentMethodsSearchService>();
        private readonly Mock<IPromotionEvaluator> _promotionEvaluatorMock = new Mock<IPromotionEvaluator>();
        private readonly Mock<IShippingMethodsSearchService> _shippingMethodsSearchServiceMock = new Mock<IShippingMethodsSearchService>();
        private readonly Mock<IShoppingCartSearchService> _shoppingCartSearchServiceMock;
        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock = new Mock<IShoppingCartService>();
        private readonly Mock<ITaxEvaluator> _taxEvaluatorMock = new Mock<ITaxEvaluator>();

        // Testable
        private readonly ShoppingCartAggregateFactory factory;

        // Constructor
        public ShoppingCartAggregateFactoryTests()
        {
            _fixture = new Fixture();
            _fixture.Register(() => new Language("en-US"));
            _fixture.Register(() => new Currency(_fixture.Create<Language>(), "USD"));
            _fixture.Register<IMutablePagedList<DynamicProperty>>(() => null);
            _fixture.Register(() => _fixture.Build<DynamicPropertyName>().With(x => x.Locale, "en-US").Create());
            _fixture.Register(() => _fixture.Build<DynamicPropertyObjectValue>().With(x => x.Locale, "en-US").Create());
            _fixture.Register<IMutablePagedList<SettingEntry>>(() => null);
            _fixture.Register<IMutablePagedList<Contact>>(() => null);
            _fixture.Register<IMutablePagedList<QuoteRequest>>(() => null);
            _fixture.Register(() => _fixture.Build<LineItem>()
                                            .Without(x => x.DynamicProperties)
                                            .With(x => x.IsReadOnly, false)
                                            .Create());
            _fixture.Register(() => _fixture.Build<ShoppingCart>().Without(x => x.DynamicProperties).Create());
            _fixture.Register<Price>(() => null);

            _shoppingCartSearchServiceMock = new Mock<IShoppingCartSearchService>();
            _shoppingCartSearchServiceMock
                .Setup(x => x.SearchCartAsync(It.IsAny<CartModule.Core.Model.Search.ShoppingCartSearchCriteria>()))
                .ReturnsAsync((CartModule.Core.Model.Search.ShoppingCartSearchCriteria criteria) =>
                {
                    return new CartModule.Core.Model.Search.ShoppingCartSearchResult
                    {
                        Results = _fixture.Build<ShoppingCart>()
                            .With(x => x.StoreId, criteria.StoreId)
                            .With(x => x.CustomerId, criteria.CustomerId)
                            .With(x => x.Name, criteria.Name)
                            .With(x => x.Type, criteria.Type)
                            .Without(x => x.DynamicProperties)
                            .CreateMany(1)
                            .ToList()
                    };
                });

            factory = new ShoppingCartAggregateFactory(_catalogServiceMock.Object,
                _paymentMethodsSearchServiceMock.Object,
                _promotionEvaluatorMock.Object,
                _shippingMethodsSearchServiceMock.Object,
                _shoppingCartSearchServiceMock.Object,
                _shoppingCartServiceMock.Object,
                _taxEvaluatorMock.Object);
        }

        [Fact]
        public async Task CreateOrGetShoppingCartAggregateAsync_ShouldCreateCartPropperly()
        {
            // Arrange
            var context = _fixture.Build<ShoppingCartContext>().Create();

            // Act
            var result = await factory.CreateOrGetShoppingCartAggregateAsync(context);

            // Assert
            result.Cart.Language.Should().Be(context.Language);
            result.Cart.Name.Should().Be(context.CartName);
            result.Cart.Currency.Code.Should().Be(context.Currency.Code);
            result.Cart.Type.Should().Be(context.Type);

            _shoppingCartSearchServiceMock
                .Verify(x => x.SearchCartAsync(It.IsAny<CartModule.Core.Model.Search.ShoppingCartSearchCriteria>()), Times.Once);

            _catalogServiceMock
                .Verify(x => x.GetProductsAsync(
                    It.IsAny<string[]>(),
                    It.IsAny<Currency>(),
                    It.IsAny<Language>(),
                    It.IsAny<ItemResponseGroup>()
                ), Times.Once);
        }
    }
}
