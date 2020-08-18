using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    public interface IStoreCurrencyResolver
    {
        Task<Currency> GetStoreCurrencyAsync(string currencyCode, string storeId, string cultureName = null);
        Task<IEnumerable<Currency>> GetAllStoreCurrenciesAsync(string storeId, string cultureName = null);
    }
}
