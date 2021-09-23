using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XPurchase.Tests.Helpers;
using Xunit;

namespace VirtoCommerce.XPurchase.Tests.Repositories
{
    public class CartAggregateRepositoryTests : XPurchaseMoqHelper
    {
        private readonly Mock<ShoppingCartSearchServiceStub> _shoppingCartSearchService;
        private readonly Mock<ShoppingCartServiceStub> _shoppingCartService;
        private readonly Mock<ICurrencyService> _currencyService;
        private readonly Mock<IStoreService> _storeService;
        private readonly Mock<IMemberResolver> _memberResolver;

        private readonly CartAggregateRepository repository;

        public CartAggregateRepositoryTests()
        {
            _shoppingCartSearchService = new Mock<ShoppingCartSearchServiceStub>();
            _shoppingCartService = new Mock<ShoppingCartServiceStub>();
            _currencyService = new Mock<ICurrencyService>();
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

        #region RemoveCartAsync

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
            _shoppingCartService.Verify(x => x.SaveChangesAsync(new List<ShoppingCart> { cartAggregate.Cart }), Times.Once);
        }

        #endregion SaveAsync

        #region GetCartAsync

        [Fact]
        public async Task GetCartAsync_ShoppingCartNotFound_ReturnNull()
        {
            // Arrange
            _shoppingCartSearchService
                .Setup(x => x.SearchAsync(It.IsAny<ShoppingCartSearchCriteria>()))
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
            _shoppingCartSearchService.Verify(x => x.SearchAsync(It.IsAny<ShoppingCartSearchCriteria>()), Times.Once);
        }

        #endregion GetCartAsync

        #region InnerGetCartAggregateFromCartAsync

        // TODO: Write tests

        #endregion InnerGetCartAggregateFromCartAsync


        public class ShoppingCartSearchServiceStub : SearchService<ShoppingCartSearchCriteria, ShoppingCartSearchResult, ShoppingCart, ShoppingCartEntity>, IShoppingCartSearchService
        {
            public ShoppingCartSearchServiceStub() : base(() => Mock.Of<ICartRepository>(), Mock.Of<IPlatformMemoryCache>(), Mock.Of<ShoppingCartServiceStub>())
            {
            }

            public virtual Task<ShoppingCartSearchResult> SearchCartAsync(ShoppingCartSearchCriteria criteria)
            {
                throw new System.NotImplementedException();
            }

            protected override IQueryable<ShoppingCartEntity> BuildQuery(IRepository repository, ShoppingCartSearchCriteria criteria)
            {
                throw new System.NotImplementedException();
            }
        }

        public class ShoppingCartServiceStub : ICrudService<ShoppingCart>, IShoppingCartService
        {
            public virtual Task DeleteAsync(IEnumerable<string> ids, bool softDelete = false)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteAsync(string[] cartIds, bool softDelete = false)
            {
                throw new System.NotImplementedException();
            }

            public Task<ShoppingCart> GetByIdAsync(string id, string responseGroup = null)
            {
                throw new System.NotImplementedException();
            }

            public Task<IEnumerable<ShoppingCart>> GetByIdsAsync(IEnumerable<string> ids, string responseGroup = null)
            {
                throw new System.NotImplementedException();
            }

            public Task<ShoppingCart[]> GetByIdsAsync(string[] cartIds, string responseGroup = null)
            {
                throw new System.NotImplementedException();
            }

            public virtual Task SaveChangesAsync(IEnumerable<ShoppingCart> models)
            {
                throw new System.NotImplementedException();
            }

            public Task SaveChangesAsync(ShoppingCart[] carts)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
