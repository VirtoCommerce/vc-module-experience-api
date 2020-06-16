using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Factories;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Customer;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Quote;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Tax;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.PricingModule.Core.Model;
using Xunit;
using DynamicProperty = VirtoCommerce.ExperienceApiModule.XPurchase.Models.DynamicProperty;

namespace VirtoCommerce.ExpirienceApiModule.XPurchase.Domain.Tests
{
    public class ShoppingCartAggregateFactoryTests
    {
        private readonly Fixture _fixture;

        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock = new Mock<IShoppingCartService>();
        private readonly Mock<ICatalogService> _catalogServiceMock = new Mock<ICatalogService>();
        private readonly Mock<IPromotionEvaluator> _promotionEvaluatorMock = new Mock<IPromotionEvaluator>();
        private readonly Mock<ITaxEvaluator> _taxEvaluatorMock = new Mock<ITaxEvaluator>();
        private readonly Mock<ICartService> _cartServiceMock = new Mock<ICartService>();
        private readonly Mock<IShoppingCartSearchService> _shoppingCartSearchServiceMock;

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

            factory = new ShoppingCartAggregateFactory(_shoppingCartServiceMock.Object,
                _catalogServiceMock.Object,
                _promotionEvaluatorMock.Object,
                _taxEvaluatorMock.Object,
                _cartServiceMock.Object,
                _shoppingCartSearchServiceMock.Object);
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

            _promotionEvaluatorMock
                .Verify(x => x.EvaluateDiscountsAsync(
                    It.IsAny<PromotionEvaluationContext>(),
                    It.IsAny<IEnumerable<IDiscountable>>()
                ), Times.Once, "Should call if no one item is read only");

            _taxEvaluatorMock
                .Verify(x => x.EvaluateTaxesAsync(
                    It.IsAny<TaxEvaluationContext>(),
                    It.IsAny<IEnumerable<ITaxable>>()
                ), Times.Once);
        }
    }
}
