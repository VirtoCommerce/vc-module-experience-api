using System;
using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.Core.Index;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadCategoryQuery : IQuery<LoadCategoryResponse>, IGetDocumentsByIdsQuery
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string[] ObjectIds { get; set; }
    }
}
