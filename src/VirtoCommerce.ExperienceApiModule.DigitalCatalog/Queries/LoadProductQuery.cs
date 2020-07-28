using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Interfaces;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadProductQuery : IQuery<LoadProductResponse>, IHasIncludeFields
    {
        public string[] Ids { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string Language { get; set; }
        public string CurrencyCode { get; set; }
        public string CartName { get; set; }
        public string Type { get; set; }
    }
}
