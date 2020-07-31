using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XDigitalCatalog.Interfaces;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQuery : IQuery<LoadCategoryResponce>, IHasIncludeFields, IGetSingleDocumentQuery
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string ObjectId { get; set; }
    }
}
