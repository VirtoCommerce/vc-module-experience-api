using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Interfaces;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public interface ICatalogQuery : IHasIncludeFields
    {
        string StoreId { get; }
        string UserId { get; }
        string CultureName { get; }
        string CurrencyCode { get; }
    }

    public class CatalogQueryBase<TResponse> : ICatalogQuery, IQuery<TResponse>
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
