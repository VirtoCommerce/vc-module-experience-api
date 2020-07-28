using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Interfaces;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQuery : IQuery<LoadCategoryResponce>, IHasIncludeFields
    {
        public string Id { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
