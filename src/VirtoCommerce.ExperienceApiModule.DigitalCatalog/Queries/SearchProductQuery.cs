using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class SearchProductQuery : IQuery<SearchProductResponse>, IHasIncludeFields
    {
        public string Query { get; set; }
        public bool Fuzzy { get; set; }
        public string Filter { get; set; }
        public string Facet { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();

    }
}
