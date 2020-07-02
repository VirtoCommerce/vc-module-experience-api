using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Handlers
{
    public class SearchProductAssociationsCommandHandler : IRequestHandler<SearchProductAssociationsCommand, SearchProductAssociationsResponse>
    {
        public SearchProductAssociationsCommandHandler()
        {
        }

        public Task<SearchProductAssociationsResponse> Handle(SearchProductAssociationsCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();

           //var result = await _searchService.SearchProductAssociationsAsync(request.Criteria);

            //return new SearchProductAssociationsResponse {  Result = result };
        }
    }
}
