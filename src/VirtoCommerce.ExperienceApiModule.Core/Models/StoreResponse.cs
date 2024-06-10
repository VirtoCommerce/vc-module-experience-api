using System.Collections.Generic;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;

namespace VirtoCommerce.ExperienceApiModule.Core.Models
{
    public class StoreResponse
    {
        public string StoreId { get; set; }

        public string StoreName { get; set; }

        public string StoreUrl { get; set; }

        public string CatalogId { get; set; }

        public Currency DefaultCurrency { get; set; }

        public IList<Currency> AvailableCurrencies { get; set; } = new List<Currency>();

        public Language DefaultLanguage { get; set; }

        public IList<Language> AvailableLanguages { get; set; } = new List<Language>();

        public StoreSettings Settings { get; set; }

        public GraphQLSettings GraphQLSettings { get; set; }
    }
}
