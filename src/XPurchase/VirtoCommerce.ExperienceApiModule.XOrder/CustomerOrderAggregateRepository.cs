using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregateRepository : ICustomerOrderAggregateRepository
    {
        private readonly Func<CustomerOrderAggregate> _customerOrderAggregateFactory;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerOrderBuilder _customerOrderBuilder;

        public CustomerOrderAggregateRepository(
            Func<CustomerOrderAggregate> customerOrderAggregateFactory,
            ICustomerOrderService customerOrderService,
            ICurrencyService currencyService,
            ICustomerOrderBuilder customerOrderBuilder)
        {
            _customerOrderAggregateFactory = customerOrderAggregateFactory;
            _customerOrderService = customerOrderService;
            _currencyService = currencyService;
            _customerOrderBuilder = customerOrderBuilder;
        }

        public async Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId)
        {
            var order = await _customerOrderService.GetByIdAsync(orderId);
            if (order != null)
            {
                var result = await InnerGetCustomerOrderAggregatesFromCustomerOrdersAsync(new[] { order });
                return result.FirstOrDefault();
            }
            return null;
        }

        public async Task<CustomerOrderAggregate> CreateOrderFromCart(ShoppingCart cart)
        {
            var response = await _customerOrderBuilder.PlaceCustomerOrderFromCartAsync(cart);
            var order = await _customerOrderService.GetByIdAsync(response.Id);
            if (order != null)
            {
                var result = await InnerGetCustomerOrderAggregatesFromCustomerOrdersAsync(new[] { order }, order.LanguageCode);
                return result.FirstOrDefault();
            }

            return null;
        }

        public async Task<CustomerOrderAggregate> GetAggregateFromOrderAsync(CustomerOrder order)
        {
            var result = await InnerGetCustomerOrderAggregatesFromCustomerOrdersAsync(new[] { order });
            return result.FirstOrDefault();
        }

        public Task<IList<CustomerOrderAggregate>> GetAggregatesFromOrdersAsync(IList<CustomerOrder> orders, string cultureName = null)
        {
            return InnerGetCustomerOrderAggregatesFromCustomerOrdersAsync(orders, cultureName);
        }

        protected virtual async Task<IList<CustomerOrderAggregate>> InnerGetCustomerOrderAggregatesFromCustomerOrdersAsync(IList<CustomerOrder> orders, string cultureName = null)
        {
            var currencies = await _currencyService.GetAllCurrenciesAsync();

            return orders.Select(x =>
            {
                var aggregate = _customerOrderAggregateFactory();
                aggregate.GrabCustomerOrder(x.Clone() as CustomerOrder, currencies.GetCurrencyForLanguage(x.Currency, cultureName ?? x.LanguageCode));
                return aggregate;
            }).ToList();
        }
    }
}
