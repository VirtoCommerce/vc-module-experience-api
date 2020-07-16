using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
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

        protected virtual async Task<CustomerOrderAggregate> InnerGetCustomerOrderAggregateFromCustomerOrderAsync(CustomerOrder order, string language = null)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();
            var currency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(order.Currency));
            if (currency == null)
            {
                throw new OperationCanceledException($"cart currency {order.Currency} is not registered in the system");
            }
            //Clone  currency with cart language
            currency = new Currency(language != null ? new Language(language) : Language.InvariantLanguage, currency.Code, currency.Name, currency.Symbol, currency.ExchangeRate)
            {
                CustomFormatting = currency.CustomFormatting
            };
            var result = new CustomerOrderAggregate(order, currency);

            return result;
        }
    }
}
