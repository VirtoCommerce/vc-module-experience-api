using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class SearchProductAssociationsCommandHandler : IRequestHandler<SearchProductAssociationsQuery, SearchProductAssociationsResponse>
    {
        public SearchProductAssociationsCommandHandler()
        {
        }

        public Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

           //var result = await _searchService.SearchProductAssociationsAsync(request.Criteria);

            //return new SearchProductAssociationsResponse {  Result = result };
        }
    }
}
