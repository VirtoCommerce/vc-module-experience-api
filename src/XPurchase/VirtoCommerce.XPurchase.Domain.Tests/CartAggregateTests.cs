using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Services;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Aggregates
{
    public class CartAggregateTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<ICartProductService> _cartProductServiceMock;
        private readonly Mock<ICurrencyService> _currencyServiceMock;
        private readonly Mock<IMarketingPromoEvaluator> _marketingPromoEvaluatorMock;
        private readonly Mock<IPaymentMethodsSearchService> _paymentMethodsSearchServiceMock;
        private readonly Mock<IShippingMethodsSearchService> _shippingMethodsSearchServiceMock;
        private readonly Mock<IShoppingCartTotalsCalculator> _shoppingCartTotalsCalculatorMock;
        private readonly Mock<IStoreService> _storeServiceMock;
        private readonly Mock<ITaxProviderSearchService> _taxProviderSearchServiceMock;

        private readonly Mock<IMapper> _mappereMock;

        private readonly CartAggregate aggregate;

        public CartAggregateTests()
        {
            //_fixture.Register(() => new Language("en-US"));
            //_fixture.Register(() => new Currency(_fixture.Create<Language>(), "USD"));
            //_fixture.Register<IMutablePagedList<DynamicProperty>>(() => null);
            //_fixture.Register(() => _fixture.Build<DynamicPropertyName>().With(x => x.Locale, "en-US").Create());
            //_fixture.Register(() => _fixture.Build<DynamicPropertyObjectValue>().With(x => x.Locale, "en-US").Create());
            //_fixture.Register<IMutablePagedList<SettingEntry>>(() => null);
            //_fixture.Register<IMutablePagedList<Contact>>(() => null);
            //_fixture.Register<IMutablePagedList<QuoteRequest>>(() => null);
            //_fixture.Register<IMutablePagedList<Category>>(() => null);
            //_fixture.Register<IMutablePagedList<Product>>(() => null);
            //_fixture.Register<IList<Product>>(() => null);
            //_fixture.Register<IList<ValidationError>>(() => null);
            //_fixture.Register<IMutablePagedList<CatalogProperty>>(() => null);
            //_fixture.Register<IMutablePagedList<ProductAssociation>>(() => null);
            //_fixture.Register<IMutablePagedList<EditorialReview>>(() => null);
            //_fixture.Register(() => _fixture.Build<LineItem>()
            //                                .Without(x => x.DynamicProperties)
            //                                .With(x => x.IsReadOnly, false)
            //                                .Create());
            //_fixture.Register(() => _fixture.Build<ShoppingCart>().Without(x => x.DynamicProperties).Create());
            _fixture.Register<Price>(() => null);

            _cartProductServiceMock = new Mock<ICartProductService>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _marketingPromoEvaluatorMock = new Mock<IMarketingPromoEvaluator>();
            _paymentMethodsSearchServiceMock = new Mock<IPaymentMethodsSearchService>();
            _shippingMethodsSearchServiceMock = new Mock<IShippingMethodsSearchService>();
            _shoppingCartTotalsCalculatorMock = new Mock<IShoppingCartTotalsCalculator>();
            _storeServiceMock = new Mock<IStoreService>();
            _taxProviderSearchServiceMock = new Mock<ITaxProviderSearchService>();

            _mappereMock = new Mock<IMapper>();

            aggregate = new CartAggregate(
                _cartProductServiceMock.Object,
                _currencyServiceMock.Object,
                _marketingPromoEvaluatorMock.Object,
                _paymentMethodsSearchServiceMock.Object,
                _shippingMethodsSearchServiceMock.Object,
                _shoppingCartTotalsCalculatorMock.Object,
                _storeServiceMock.Object,
                _taxProviderSearchServiceMock.Object,
                _mappereMock.Object);
        }

        #region TakeCartAsync

        [Fact]
        public void TakeCartAsync_ShouldThrowArgumentNullException_IfCartIsNull()
        {
            // Arrange
            ShoppingCart shoppingCart = null;

            // Act
            Action action = () => aggregate.TakeCartAsync(shoppingCart).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>("Shopping cart is null");
        }

        [Fact]
        public async Task TakeCartAsync_ShouldSaveProductsToAggregate_IfCartContainsProductsAsync()
        {
            // Arrange
            var products = _fixture.Build<LineItem>()
                .With(x => x.IsReadOnly, true)
                .CreateMany()
                .ToList();

            var productIds = products.Select(x => x.ProductId).ToArray();

            var cartProducts = productIds
                .Select(productId => new CartProduct(new CatalogProduct
                {
                    Id = productId
                }))
                .ToList();

            _cartProductServiceMock
                .Setup(x => x.GetCartProductsByIdsAsync(It.IsAny<CartAggregate>(), productIds))
                .ReturnsAsync(cartProducts);

            var shoppingCart = _fixture.Build<ShoppingCart>()
                .With(x => x.Items, products)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);

            // Assert
            aggregate.CartProductsDict.Should().NotBeNull();
            aggregate.CartProductsDict.Select(x => x.Key).Should().BeSubsetOf(productIds);
        }

        [Fact]
        public async Task TakeCartAsync_ShouldAlwaysCalculateTotals()
        {
            // Arrange
            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .Without(x => x.Items)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);

            // Assert
            _shoppingCartTotalsCalculatorMock.Verify(x => x.CalculateTotals(It.IsAny<ShoppingCart>()), Times.Once);
        }

        [Fact]
        public async Task TakeCartAsync_ShouldAlwaysSaveCartToAggregate()
        {
            // Arrange
            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .Without(x => x.Items)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);

            // Assert
            aggregate.Cart.Should().NotBeNull();
            aggregate.Cart.Should().BeEquivalentTo(shoppingCart);
        }

        [Fact]
        public async Task TakeCartAsync_ShouldAlwaysSaveIdToAggregate()
        {
            // Arrange
            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .Without(x => x.Items)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);

            // Assert
            aggregate.Id.Should().Be(shoppingCart.Id);
        }

        #endregion TakeCartAsync

        #region UpdateCartComment

        [Fact]
        public void UpdateCartComment_ShouldThrowOperationCanceledException_IfCartNotLoaded()
        {
            // Arrange
            var comment = _fixture.Create<string>();

            // Act
            Action action = () => aggregate.UpdateCartComment(comment).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<OperationCanceledException>("Cart not loaded");
        }

        [Fact]
        public async Task UpdateCartComment_ShouldSaveCommentToAggregate()
        {
            // Arrange
            var comment = _fixture.Create<string>();
            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .Without(x => x.Items)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);
            await aggregate.UpdateCartComment(comment);

            // Assert
            aggregate.Cart.Should().NotBeNull();
            aggregate.Cart.Comment.Should().Be(comment);
        }

        #endregion UpdateCartComment

        #region AddItemAsync

        [Fact]
        public void AddItemAsync_ShouldThrowOperationCanceledException_IfCartNotLoaded()
        {
            // Arrange
            NewCartItem newCartItem = null;

            // Act
            Action action = () => aggregate.AddItemAsync(newCartItem).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<OperationCanceledException>("Cart not loaded");
        }

        [Fact]
        public async Task AddItemAsync_ShouldThrowArgumentNullException_IfNewCartItemIsNullAsync()
        {
            // Arrange
            NewCartItem newCartItem = null;

            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .Without(x => x.Items)
                .Create();

            // Act
            await aggregate.TakeCartAsync(shoppingCart);
            Action action = () => aggregate.AddItemAsync(newCartItem).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>("NewCartItem is null");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task AddItemAsync_ShouldThrow_IfQuantityLessOrEqualZero(int quantity)
        {
            // Arrange
            var productId = _fixture.Create<string>();
            var newCartItem = new NewCartItem(productId, quantity);

            var shoppingCart = _fixture
                .Build<ShoppingCart>()
                .With(x => x.Items, Enumerable.Empty<LineItem>().ToList())
                .Create();

            _cartProductServiceMock
                .Setup(x => x.GetCartProductsByIdsAsync(It.IsAny<CartAggregate>(), new[] { productId }))
                .ReturnsAsync(new List<CartProduct>() { new CartProduct(new CatalogProduct()) });

            // Act
            await aggregate.TakeCartAsync(shoppingCart);
            Action action = () => aggregate.AddItemAsync(newCartItem).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ValidationException>($"Quantity is {quantity}");
        }

        #endregion AddItemAsync
    }
}
