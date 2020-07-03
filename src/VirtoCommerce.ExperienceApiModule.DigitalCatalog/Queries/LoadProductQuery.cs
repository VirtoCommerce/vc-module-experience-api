using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class LoadProductQuery : IQuery<LoadProductResponse>, IHasIncludeFields
    {
        public string[] Ids { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
