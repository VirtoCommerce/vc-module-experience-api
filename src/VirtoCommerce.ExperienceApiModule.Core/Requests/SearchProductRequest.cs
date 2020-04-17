using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class SearchProductRequest : IRequest<SearchProductResponse>, IHasIncludeFields
    {
        public ProductIndexedSearchCriteria Criteria { get; set; }
        public IEnumerable<string> IncludeFields { get => Criteria?.IncludeFields; set => Criteria.IncludeFields = value?.ToArray(); }
    }
}
