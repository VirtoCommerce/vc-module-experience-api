using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog
{
    //TODO: Move to Store module
    public class StoreCurrencyResolver : IStoreCurrencyResolver
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStoreService _storeService;
        public StoreCurrencyResolver(
            ICurrencyService currencyService
            , IStoreService storeService
        )
        {          
            _currencyService = currencyService;
            _storeService = storeService;
        }


        public async Task<IEnumerable<Currency>> GetAllStoreCurrenciesAsync(string storeId, string cultureName = null)
        {
            if (storeId == null)
            {
                throw new ArgumentNullException(nameof(storeId));
            }
            var store = await _storeService.GetByIdAsync(storeId);
            var defaultCultureName = store.DefaultLanguage ?? Language.InvariantLanguage.CultureName;
            //Clone currencies
            //TODO: Add caching  to prevent cloning each time
            var allCurrencies = (await _currencyService.GetAllCurrenciesAsync()).Select(x => x.Clone()).OfType<Currency>().ToArray();
            //Change culture name for all system currencies to requested
            allCurrencies.Apply(x => x.CultureName = cultureName ?? defaultCultureName);

            return allCurrencies;
        }

        public async Task<Currency> GetStoreCurrencyAsync(string currencyCode, string storeId, string cultureName = null)
        {
            var store = await _storeService.GetByIdAsync(storeId);

            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                currencyCode = store.DefaultCurrency;
            }

            var allCurrencies = await GetAllStoreCurrenciesAsync(storeId, cultureName);
            //Clone and change culture name for system currencies
            var currency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(currencyCode));
            if (currency == null)
            {
                throw new OperationCanceledException($"requested currency {currencyCode} is not registered in the system");
            }

            return currency;
        }
    }
}
