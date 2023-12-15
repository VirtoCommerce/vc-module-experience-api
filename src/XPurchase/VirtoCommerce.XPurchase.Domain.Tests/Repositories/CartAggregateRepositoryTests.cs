using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Repositories
{
    public class CartAggregateRepositoryTests : XPurchaseMoqHelper
    {
        private readonly Mock<IShoppingCartSearchService> _shoppingCartSearchService;
        private readonly Mock<IShoppingCartService> _shoppingCartService;
        private readonly Mock<ICurrencyService> _currencyService;
        private readonly Mock<IStoreService> _storeService;
        private readonly Mock<IMemberResolver> _memberResolver;

        private readonly CartAggregateRepository repository;

        public CartAggregateRepositoryTests()
        {
            _shoppingCartSearchService = new Mock<IShoppingCartSearchService>();
            _shoppingCartService = new Mock<IShoppingCartService>();
            _currencyService = new Mock<ICurrencyService>();
            _storeService = new Mock<IStoreService>();
            _memberResolver = new Mock<IMemberResolver>();

            repository = new CartAggregateRepository(
                () => _fixture.Create<CartAggregate>(),
                _shoppingCartSearchService.Object,
                _shoppingCartService.Object,
                _currencyService.Object,
                _memberResolver.Object,
                _storeService.Object,
                null
                );
        }

        [Fact]
        public async Task RemoveCartAsync_ShouldCallShoppingCart()
        {
            // Arrange
            var cartId = _fixture.Create<string>();

            // Act
            await repository.RemoveCartAsync(cartId);

            // Assert
            _shoppingCartService.Verify(x => x.DeleteAsync(new List<string> { cartId }, It.IsAny<bool>()), Times.Once);
        }

        /// <summary>
        /// If this test fails check GetValidCartAggregate() from MoqHelper
        /// </summary>
        [Fact]
        public async Task SaveAsync_ShouldCallShoppingCart()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();

            // Act
            await repository.SaveAsync(cartAggregate);

            // Assert
            _shoppingCartService.Verify(x => x.SaveChangesAsync(new List<ShoppingCart> { cartAggregate.Cart }), Times.Once);
        }

        [Fact]
        public async Task GetCartAsync_ShoppingCartNotFound_ReturnNull()
        {
            // Arrange
            _shoppingCartSearchService
                .Setup(x => x.SearchAsync(It.IsAny<ShoppingCartSearchCriteria>(), It.IsAny<bool>()))
                .ReturnsAsync(new ShoppingCartSearchResult
                {
                    Results = new List<ShoppingCart>()
                });

            // Act
            var result = await repository.GetCartAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>());

            // Assert
            result.Should().BeNull();
            _shoppingCartSearchService.Verify(x => x.SearchAsync(It.IsAny<ShoppingCartSearchCriteria>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_CartFound_AggregateReturnedCorrectly()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();

            var repository = new CartAggregateRepository(
                 () => cartAggregate,
                 _shoppingCartSearchService.Object,
                 _shoppingCartService.Object,
                 _currencyService.Object,
                 _memberResolver.Object,
                 _storeService.Object,
                 _cartProductServiceMock.Object
                 );

            var storeId = "Store";
            var store = _fixture.Create<Store>();
            store.Id = storeId;

            var shoppingCart = _fixture.Create<ShoppingCart>();
            shoppingCart.StoreId = storeId;

            _storeService.Setup(x => x.GetAsync(new[] { storeId }, It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new[] { store });

            var currencies = _fixture.CreateMany<Currency>(1).ToList();

            _currencyService.Setup(x => x.GetAllCurrenciesAsync())
                .ReturnsAsync(currencies);

            var customer = _fixture.Create<Contact>();
            _memberResolver.Setup(x => x.ResolveMemberByIdAsync(It.Is<string>(x => x == shoppingCart.CustomerId)))
                .ReturnsAsync(customer);

            _cartProductServiceMock.Setup(x => x.GetCartProductsByIdsAsync(It.Is<CartAggregate>(x => x == cartAggregate), It.IsAny<IList<string>>()))
                .ReturnsAsync(new List<CartProduct>());

            // Act
            var result = await repository.GetCartForShoppingCartAsync(shoppingCart);

            // Assert
            result.Id.Should().Be(shoppingCart.Id);
            result.Cart.Should().Be(shoppingCart);
            result.Member.Should().Be(customer);
            result.Store.Should().Be(store);
            result.Currency.Code.Should().Be(currencies.FirstOrDefault().Code);
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_ProductPriceChanged_ShouldContainWarnings()
        {
            // Arrange
            var cartAggregate = GetValidCartAggregate();

            var repository = new CartAggregateRepository(
                 () => cartAggregate,
                 _shoppingCartSearchService.Object,
                 _shoppingCartService.Object,
                 _currencyService.Object,
                 _memberResolver.Object,
                 _storeService.Object,
                 _cartProductServiceMock.Object);

            var storeId = "Store";
            var store = _fixture.Create<Store>();
            store.Id = storeId;

            var shoppingCart = _fixture.Create<ShoppingCart>();
            var lineItem = _fixture.Create<LineItem>();
            shoppingCart.Items = new List<LineItem>() { lineItem };
            shoppingCart.StoreId = storeId;

            _storeService.Setup(x => x.GetAsync(new[] { storeId }, It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new[] { store });

            var currencies = _fixture.CreateMany<Currency>(1).ToList();

            _currencyService.Setup(x => x.GetAllCurrenciesAsync())
                .ReturnsAsync(currencies);

            var customer = _fixture.Create<Contact>();
            _memberResolver.Setup(x => x.ResolveMemberByIdAsync(It.Is<string>(x => x == shoppingCart.CustomerId)))
                .ReturnsAsync(customer);

            _cartProductServiceMock.Setup(x => x.GetCartProductsByIdsAsync(It.Is<CartAggregate>(x => x == cartAggregate), It.IsAny<IList<string>>()))
                .ReturnsAsync(() =>
                {
                    var product = _fixture.Create<CartProduct>();
                    product.Id = lineItem.ProductId;

                    //change price
                    product.ApplyPrices(new List<Price>()
                    {
                        new Price
                        {
                            ProductId = product.Id,
                            PricelistId = _fixture.Create<string>(),
                            List = 1,
                            MinQuantity = 1,
                        }
                    }, GetCurrency());

                    return new List<CartProduct>() { product };
                });

            // Act
            var result = await repository.GetCartForShoppingCartAsync(shoppingCart);

            // Assert
            result.ValidationWarnings.Should().HaveCount(1);
        }
    }
}
