using System.Collections.Generic;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Index
{
    public interface IRequestBuilder
    {
        IRequestBuilder FromQuery(ISearchDocumentsQuery query);

        IRequestBuilder FromQuery(IGetDocumentsByIdsQuery query);

        IRequestBuilder AddTerms(IEnumerable<string> terms);

        IRequestBuilder ParseFacets(string facetPhrase, string storeId = null, string currency = null);

        SearchRequest Build();
    }
}
