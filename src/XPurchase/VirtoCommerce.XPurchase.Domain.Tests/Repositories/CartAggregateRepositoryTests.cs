using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using VirtoCommerce.XPurchase.Validators;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Repositories
{
    public class CartAggregateRepositoryTests : MoqHelper
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<ICartValidationContextFactory> _cartValidationContextFactory;
        private readonly Mock<IShoppingCartSearchService> _shoppingCartSearchService;
        private readonly Mock<IShoppingCartService> _shoppingCartService;
        private readonly Mock<ICurrencyService> _currencyService;
        private readonly Mock<IMemberService> _memberService;
        private readonly Mock<IStoreService> _storeService;

        private readonly CartAggregateRepository repository;

        public CartAggregateRepositoryTests()
        {
            _cartValidationContextFactory = new Mock<ICartValidationContextFactory>();
            _shoppingCartSearchService = new Mock<IShoppingCartSearchService>();
            _shoppingCartService = new Mock<IShoppingCartService>();
            _currencyService = new Mock<ICurrencyService>();
            _memberService = new Mock<IMemberService>();
            _storeService = new Mock<IStoreService>();

            repository = new CartAggregateRepository(
                () => _fixture.Create<CartAggregate>(),
                _shoppingCartSearchService.Object,
                _shoppingCartService.Object,
                _currencyService.Object,
                _memberService.Object,
                _storeService.Object,
                _cartValidationContextFactory.Object,
                () => TestUserManager<ApplicationUser>()
                );
        }

        #region GetCartForShoppingCartAsync

        [Fact]
        public void GetCartForShoppingCartAsync_ShouldThrowArgumentNullException_IfCartIsNull()
        {
            // Arrange
            ShoppingCart shoppingCart = null;

            // Act
            Action action = () => repository.GetCartForShoppingCartAsync(shoppingCart).GetAwaiter().GetResult();

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>("Shopping cart is null");
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_ShouldSaveProductsToAggregate_IfCartContainsProductsAsync()
        {
            // Arrange
            //var products = _fixture.CreateMany<LineItem>().ToList();

            //var productIds = products.Select(x => x.ProductId).ToArray();

            //var cartProducts = productIds
            //    .Select(productId => new CartProduct(new CatalogProduct
            //    {
            //        Id = productId
            //    }))
            //    .ToList();

            //_cartProductServiceMock
            //    .Setup(x => x.GetCartProductsByIdsAsync(It.IsAny<CartAggregate>(), productIds))
            //    .ReturnsAsync(cartProducts);

            //var shoppingCart = _fixture.Create<ShoppingCart>();
            //shoppingCart.Items = products;

            //// Act
            //var aggregate = await repository.GetCartForShoppingCartAsync(shoppingCart);

            //// Assert
            //aggregate.CartProducts.Should().NotBeNull();
            //aggregate.CartProducts.Select(x => x.Key).Should().BeSubsetOf(productIds);
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_ShouldAlwaysCalculateTotals()
        {
            //// Arrange
            //var shoppingCart = _fixture.Create<ShoppingCart>();

            //// Act
            //await repository.GetCartForShoppingCartAsync(shoppingCart);

            //// Assert
            //_shoppingCartTotalsCalculatorMock.Verify(x => x.CalculateTotals(It.IsAny<ShoppingCart>()), Times.Once);
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_ShouldAlwaysSaveCartToAggregate()
        {
            //// Arrange
            //var shoppingCart = _fixture.Create<ShoppingCart>();

            //// Act
            //await repository.GetCartForShoppingCartAsync(shoppingCart);

            //// Assert
            //repository.Cart.Should().NotBeNull();
            //repository.Cart.Should().BeEquivalentTo(shoppingCart);
        }

        [Fact]
        public async Task GetCartForShoppingCartAsync_ShouldAlwaysSaveIdToAggregate()
        {
            //// Arrange
            //var shoppingCart = _fixture.Create<ShoppingCart>();

            //// Act
            //await repository.GetCartForShoppingCartAsync(shoppingCart);

            //// Assert
            //repository.Id.Should().Be(shoppingCart.Id);
        }

        #endregion GetCartForShoppingCartAsync
    }
}
