using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class SearchProductAssociationsHandler : IRequestHandler<SearchProductAssociationsRequest, SearchProductAssociationsResponse>
    {
        public SearchProductAssociationsHandler()
        {
        }

        public Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

           //var result = await _searchService.SearchProductAssociationsAsync(request.Criteria);

            //return new SearchProductAssociationsResponse {  Result = result };
        }
    }
}
