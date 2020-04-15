using System;
using System.Collections.Generic;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class SearchProductRequest : IRequest<SearchProductResponse>, IHasIncludeFields
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public ProductSearchCriteria Criteria { get; set; }
    }
}
