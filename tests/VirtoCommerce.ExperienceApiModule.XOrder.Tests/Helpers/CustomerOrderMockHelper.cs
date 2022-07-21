using AutoFixture;
using Moq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;
using VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers.Stubs;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Model;
using Cart = VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Tests.Helpers
{
    public class CustomerOrderMockHelper : MoqHelper
    {
        protected readonly Mock<IDynamicPropertyUpdaterService> _dynamicPropertyUpdaterService;
        protected readonly Mock<IPromotionUsageSearchService> _promotionUsageSearchService;

        protected const int InStockQuantity = 1;
        protected const int ItemCost = 50;

        public CustomerOrderMockHelper()
        {
            _fixture.Register<PaymentMethod>(() => new StubPaymentMethod(_fixture.Create<string>()));

            _fixture.Register(() => _fixture.Build<PaymentIn>()
                .Without(x => x.DynamicProperties)
                .Without(x => x.ChildrenOperations)
                .Create());

            _fixture.Register(() => _fixture.Build<Shipment>()
                .Without(x => x.DynamicProperties)
                .Without(x => x.ChildrenOperations)
                .Create());

            _fixture.Register(() => _fixture.Build<LineItem>()
                .Without(x => x.DynamicProperties)
                .Create());

            _fixture.Register(() => _fixture
                .Build<CustomerOrder>()
                .With(x => x.Currency, CURRENCY_CODE)
                .With(x => x.LanguageCode, CULTURE_NAME)
                .Without(x => x.Items)
                .Without(x => x.Shipments)
                .Without(x => x.ChildrenOperations)
                .Without(x => x.DynamicProperties)
                .Create());

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
                _fixture.Build<Optional<ExpOrderAddress>>()
               .With(x => x.IsSpecified, true)
               .Create());

            _fixture.Register(() => _fixture
                .Build<Cart.ShoppingCart>()
                .With(x => x.Currency, CURRENCY_CODE)
                .With(x => x.LanguageCode, CULTURE_NAME)
                .With(x => x.Name, "default")
                .Without(x => x.Items)
                .Without(x => x.Shipments)
                .Without(x => x.Payments)
                .Create());
            _fixture.Register(() => _fixture.Build<Cart.LineItem>()
                                .Without(x => x.DynamicProperties)
                                .With(x => x.IsReadOnly, false)
                                .With(x => x.IsGift, false)
                                .With(x => x.Quantity, InStockQuantity)
                                .With(x => x.SalePrice, ItemCost)
                                .With(x => x.ListPrice, ItemCost)
                                .Create());
            _fixture.Register<Price>(() => null);

            _dynamicPropertyUpdaterService = new Mock<IDynamicPropertyUpdaterService>();
            _promotionUsageSearchService = new Mock<IPromotionUsageSearchService>();
        }

        protected CustomerOrder GetCustomerOrder() => _fixture.Create<CustomerOrder>();

        protected CustomerOrderAggregate GetOrderAggregate(CustomerOrder customerOrder = null)
        {
            customerOrder ??= GetCustomerOrder();

            var currency = new Currency(_fixture.Create<Language>(), customerOrder.Currency);

            var aggregate = new CustomerOrderAggregate(
                _dynamicPropertyUpdaterService.Object,
                _promotionUsageSearchService.Object);

            aggregate.GrabCustomerOrder(customerOrder, currency);

            return aggregate;
        }
    }
}
