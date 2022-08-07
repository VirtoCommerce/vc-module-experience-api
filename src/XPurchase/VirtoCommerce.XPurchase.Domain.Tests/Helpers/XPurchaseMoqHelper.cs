using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using Bogus;
using Moq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Tests.Helpers.Stubs;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase.Tests.Helpers
{
    public class XPurchaseMoqHelper : MoqHelper
    {
        // For Validators
        protected readonly CartValidationContext _context = new CartValidationContext();

        protected readonly Mock<ICartProductService> _cartProductServiceMock;
        protected readonly Mock<ICurrencyService> _currencyServiceMock;
        protected readonly Mock<IMarketingPromoEvaluator> _marketingPromoEvaluatorMock;
        protected readonly Mock<IPaymentMethodsSearchService> _paymentMethodsSearchServiceMock;
        protected readonly Mock<IShippingMethodsSearchService> _shippingMethodsSearchServiceMock;
        protected readonly Mock<IShoppingCartTotalsCalculator> _shoppingCartTotalsCalculatorMock;
        protected readonly Mock<IStoreService> _storeServiceMock;
        protected readonly Mock<ITaxProviderSearchService> _taxProviderSearchServiceMock;
        protected readonly Mock<IDynamicPropertyUpdaterService> _dynamicPropertyUpdaterService;
        protected readonly Mock<IMapper> _mapperMock;
        protected readonly Mock<IMemberOrdersService> _memberOrdersServiceMock;
        protected readonly Mock<SettingsExtensions> _settingsExtensionsMock;

        protected readonly Randomizer Rand = new Randomizer();

        private const string CART_NAME = "default";

        protected const int InStockQuantity = 100;
        protected const int ItemCost = 50;

        public XPurchaseMoqHelper()
        {
            _fixture.Register<PaymentMethod>(() => new StubPaymentMethod(_fixture.Create<string>()));

            _fixture.Register(() => _fixture
                .Build<ShoppingCart>()
                .With(x => x.Currency, CURRENCY_CODE)
                .With(x => x.LanguageCode, CULTURE_NAME)
                .With(x => x.Name, CART_NAME)
                .Without(x => x.Items)
                .Create());

            _fixture.Register(() =>
            {
                var catalogProduct = _fixture.Create<CatalogProduct>();

                catalogProduct.TrackInventory = true;

                var cartProduct = new CartProduct(catalogProduct);

                cartProduct.ApplyPrices(new List<Price>()
                {
                    new Price
                    {
                        ProductId = catalogProduct.Id,
                        PricelistId = _fixture.Create<string>(),
                        List = ItemCost,
                        MinQuantity = 1,
                    }
                }, GetCurrency());

                var store = GetStore();

                cartProduct.ApplyInventories(new List<InventoryInfo>()
                {
                    new InventoryInfo
                    {
                        ProductId=catalogProduct.Id,
                        FulfillmentCenterId = store.MainFulfillmentCenterId,
                        InStockQuantity = InStockQuantity,
                        ReservedQuantity = 0,
                    }
                }, store);

                return cartProduct;
            });

            _fixture.Register(() => new CatalogProduct
            {
                Id = _fixture.Create<string>(),
                IsActive = true,
                IsBuyable = true,
            });

            _fixture.Register(() => _fixture.Build<LineItem>()
                                            .Without(x => x.DynamicProperties)
                                            .With(x => x.IsReadOnly, false)
                                            .With(x => x.IsGift, false)
                                            .With(x => x.Quantity, InStockQuantity)
                                            .With(x => x.SalePrice, ItemCost)
                                            .With(x => x.ListPrice, ItemCost)
                                            .Create());

            _fixture.Register<Price>(() => null);

            _fixture.Register(() =>
                _fixture.Build<Optional<string>>()
                .With(x => x.IsSpecified, true)
                .Create());

            _fixture.Register(() =>
                _fixture.Build<Optional<int>>()
                .With(x => x.IsSpecified, true)
                .Create());

            _fixture.Register(() =>
                _fixture.Build<Optional<decimal>>()
                .With(x => x.IsSpecified, true)
                .Create());

            _fixture.Register(() =>
                _fixture.Build<Optional<decimal?>>()
                .With(x => x.IsSpecified, true)
                .Create());

            _fixture.Register(() =>
                _fixture.Build<Optional<ExpCartAddress>>()
               .With(x => x.IsSpecified, true)
               .Create());

            _cartProductServiceMock = new Mock<ICartProductService>();

            _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyServiceMock
                .Setup(x => x.GetAllCurrenciesAsync())
                .ReturnsAsync(_fixture.CreateMany<Currency>(1).ToList());

            _marketingPromoEvaluatorMock = new Mock<IMarketingPromoEvaluator>();
            _marketingPromoEvaluatorMock
                .Setup(x => x.EvaluatePromotionAsync(It.IsAny<IEvaluationContext>()))
                .ReturnsAsync(new StubPromotionResult());

            _paymentMethodsSearchServiceMock = new Mock<IPaymentMethodsSearchService>();
            _shippingMethodsSearchServiceMock = new Mock<IShippingMethodsSearchService>();
            _shoppingCartTotalsCalculatorMock = new Mock<IShoppingCartTotalsCalculator>();

            _storeServiceMock = new Mock<IStoreService>();
            _storeServiceMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<Store>());

            _taxProviderSearchServiceMock = new Mock<ITaxProviderSearchService>();
            _dynamicPropertyUpdaterService = new Mock<IDynamicPropertyUpdaterService>();

            _mapperMock = new Mock<IMapper>();

            _memberOrdersServiceMock = new Mock<IMemberOrdersService>();
            _memberOrdersServiceMock
                .Setup(x => x.IsFirstTimeBuyer(It.IsAny<string>()))
                .Returns(true);

            _settingsExtensionsMock = new Mock<SettingsExtensions>();
        }

        protected ShoppingCart GetCart() => _fixture.Create<ShoppingCart>();

        protected Member GetMember() => _fixture.Create<StubMember>();

        protected Store GetStore() => _fixture.Create<Store>();

        protected NewCartItem BuildNewCartItem(
            string productId,
            int quantity,
            decimal productPrice,
            bool? isActive = null,
            bool? isBuyable = null,
            bool? trackInventory = null)
        {
            var catalogProductId = _fixture.Create<string>();

            var catalogProduct = new CatalogProduct
            {
                Id = catalogProductId,
                IsActive = isActive,
                IsBuyable = isBuyable,
                TrackInventory = trackInventory
            };

            var cartProduct = new CartProduct(catalogProduct);
            cartProduct.ApplyPrices(new List<Price>()
            {
                new Price
                {
                    ProductId = catalogProductId,
                    PricelistId = _fixture.Create<string>(),
                    List = _fixture.Create<decimal>(),
                    MinQuantity = _fixture.Create<int>(),
                }
            }, GetCurrency());

            var newCartItem = new NewCartItem(productId, quantity)
            {
                Price = productPrice,
                CartProduct = cartProduct
            };

            return newCartItem;
        }

        protected CartAggregate GetValidCartAggregate()
        {
            var cart = GetCart();

            var aggregate = new CartAggregate(
                _marketingPromoEvaluatorMock.Object,
                _shoppingCartTotalsCalculatorMock.Object,
                _taxProviderSearchServiceMock.Object,
                _cartProductServiceMock.Object,
                _dynamicPropertyUpdaterService.Object,
                _mapperMock.Object,
                _memberOrdersServiceMock.Object,
                _settingsExtensionsMock.Object);

            aggregate.GrabCart(cart, new Store(), GetMember(), GetCurrency());

            return aggregate;
        }
    }
}
