using System;
using System.Collections.Generic;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.Core.Requests
{
    public class SearchProductRequest : IRequest<SearchProductResponse>, IHasIncludeFields
    {
        //
        // Summary:
        //     Term format: name:value1,value2
        public IList<string> Terms { get; set; }
        public string CatalogId { get; set; }
        public IList<string> ObjectIds { get; set; }
        public string Keyword { get; set; }
        public string LanguageCode { get; set; }
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
    }
}
