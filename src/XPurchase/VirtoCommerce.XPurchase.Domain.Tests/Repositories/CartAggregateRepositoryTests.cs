using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
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
            _cartValidationContextFactory
                .Setup(x => x.CreateValidationContextAsync(It.IsAny<CartAggregate>()))
                .ReturnsAsync(new CartValidationContext());

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

        #region RemoveCartAsync

        [Fact]
        public async Task RemoveCartAsync_ShouldCallShoppingCart()
        {
            // Arrange
            var cartId = _fixture.Create<string>();

            // Act
            await repository.RemoveCartAsync(cartId);

            // Assert
            _shoppingCartService.Verify(x => x.DeleteAsync(new[] { cartId }, It.IsAny<bool>()), Times.Once);
        }

        #endregion RemoveCartAsync

        #region SaveAsync

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
            _shoppingCartService.Verify(x => x.SaveChangesAsync(new ShoppingCart[] { cartAggregate.Cart }), Times.Once);
        }

        #endregion SaveAsync

        #region GetCartAsync

        [Fact]
        public async Task GetCartAsync_ShoppingCartNotFound_ReturnNull()
        {
            // Arrange
            _shoppingCartSearchService
                .Setup(x => x.SearchCartAsync(It.IsAny<ShoppingCartSearchCriteria>()))
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
            _shoppingCartSearchService.Verify(x => x.SearchCartAsync(It.IsAny<ShoppingCartSearchCriteria>()), Times.Once);
        }

        #endregion GetCartAsync

        #region InnerGetCartAggregateFromCartAsync

        // TODO: Write tests

        #endregion InnerGetCartAggregateFromCartAsync
    }
}
