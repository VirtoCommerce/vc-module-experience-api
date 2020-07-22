using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchCategoryQuery : IQuery<SearchCategoryResponse>, IHasIncludeFields
    {
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string Lang { get; set; }
        public string Currency { get; set; }

        public string Query { get; set; }
        public bool Fuzzy { get; set; }
        public int? FuzzyLevel { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public IEnumerable<string> CategoryIds { get; set; }
    }
}
