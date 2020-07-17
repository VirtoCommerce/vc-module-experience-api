using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Queries;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder
{
    public class CustomerOrderAggregateRepository : ICustomerOrderAggregateRepository
    {
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerOrderSearchService _customerOrderSearchService;
        private readonly ICurrencyService _currencyService;

        public CustomerOrderAggregateRepository(ICustomerOrderService customerOrderService, ICustomerOrderSearchService customerOrderSearchService, ICurrencyService currencyService)
        {
            _customerOrderService = customerOrderService;
            _customerOrderSearchService = customerOrderSearchService;
            _currencyService = currencyService;
        }

        public async Task<CustomerOrderAggregate> GetOrderByIdAsync(string orderId)
        {
            var order = await _customerOrderService.GetByIdAsync(orderId);
            var result = await InnerGetCustomerOrderAggregateFromCustomerOrderAsync(order, order.LanguageCode);
            return result;
        }

        public async Task<CustomerOrderAggregate> GetOrderByNumberAsync(string number)
        {
            var order = (await _customerOrderSearchService.SearchCustomerOrdersAsync(new CustomerOrderSearchCriteria { Number = number })).Results.FirstOrDefault();
            var result = await InnerGetCustomerOrderAggregateFromCustomerOrderAsync(order, order.LanguageCode);

            return result;
        }

        public async Task<IList<CustomerOrderAggregate>> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria searchCriteria)
        {
            var response = await _customerOrderSearchService.SearchCustomerOrdersAsync(searchCriteria);

            return await InnerGetCustomerOrdersAggregateFromCustomerOrderAsync(response.Results);
        }

        protected virtual async Task<CustomerOrderAggregate> InnerGetCustomerOrderAggregateFromCustomerOrderAsync(CustomerOrder order, string language = null)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var currency = (await GetCurrenciesAsync(new[] { order.Currency })).FirstOrDefault();

            if (currency == null)
            {
                throw new OperationCanceledException($"order currency {currency} is not registered in the system");
            }
            //Clone  currency with cart language
            currency = new Currency(language != null ? new Language(language) : Language.InvariantLanguage, currency.Code, currency.Name, currency.Symbol, currency.ExchangeRate)
            {
                CustomFormatting = currency.CustomFormatting
            };

            var result = new CustomerOrderAggregate(order, currency);

            return result;
        }

        protected virtual async Task<IList<CustomerOrderAggregate>> InnerGetCustomerOrdersAggregateFromCustomerOrderAsync(IList<CustomerOrder> orders, string language = null)
        {
            var currencies = await GetCurrenciesAsync(orders.Select(x => x.Currency).Distinct().ToArray());

            return orders.Select(x => new CustomerOrderAggregate(x, currencies.FirstOrDefault(c => c.Code.EqualsInvariant(x.Currency)))).ToList();
        }

        private async Task<Currency[]> GetCurrenciesAsync(string[] currencyCodes)
        {
            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            return allCurrencies.Where(x => currencyCodes.Contains(x.Code)).ToArray();
        }
    }
}
