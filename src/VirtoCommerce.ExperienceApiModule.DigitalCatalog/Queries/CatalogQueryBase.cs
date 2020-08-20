using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class CatalogQueryBase<TResponse> : ICatalogQuery, IQuery<TResponse>
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
