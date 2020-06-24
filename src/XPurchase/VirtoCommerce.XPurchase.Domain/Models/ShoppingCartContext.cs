using System.Collections.Generic;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.OperationResults;
using VirtoCommerce.XPurchase.Models.Stores;

namespace VirtoCommerce.XPurchase.Domain.Models
{
    public class ShoppingCartContext
    {
        public string StoreId { get; set; }
        public string CartName { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }
        public string Type { get; set; }

        public Store Store { get; private set; } = new Store();

        public void SetStore(Store store) => Store = store; //todo if need

        public Language Language { get; private set; } = new Language("en-us");

        public void SetLanguage(Language language) => Language = language;

        public Currency Currency { get; private set; } = new Currency(new Language("en-us"), "USD");

        public void SetCurrency(Currency currency) => Currency = currency;

        public List<ErrorResult> InitializationErrors { get; set; } = new List<ErrorResult>();

        public ContextState InitializationStatus { get; set; } = ContextState.NotInitialized;

        public enum ContextState
        {
            NotInitialized,
            Initialized
        }
    }
}
