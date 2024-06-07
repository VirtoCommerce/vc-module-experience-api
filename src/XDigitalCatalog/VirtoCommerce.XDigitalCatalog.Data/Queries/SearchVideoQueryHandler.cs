using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.XDigitalCatalog.Core.Models;
using VirtoCommerce.XDigitalCatalog.Core.Queries;

namespace VirtoCommerce.XDigitalCatalog.Data.Queries
{
    public class SearchVideoQueryHandler : IRequestHandler<SearchVideoQuery, SearchVideoQueryResponse>
    {
        private readonly IVideoSearchService _videoSearchService;

        public SearchVideoQueryHandler(IVideoSearchService videoSearchService)
        {
            _videoSearchService = videoSearchService;
        }

        public async Task<SearchVideoQueryResponse> Handle(SearchVideoQuery request, CancellationToken cancellationToken)
        {
            var criteria = new VideoSearchCriteria
            {
                Take = request.Take,
                Skip = request.Skip,
                LanguageCode = request.CultureName,
                OwnerIds = !string.IsNullOrEmpty(request.OwnerId) ? new List<string> { request.OwnerId } : null,
                OwnerType = request.OwnerType,
            };

            var result = await _videoSearchService.SearchAsync(criteria);

            return new SearchVideoQueryResponse
            {
                Result = result,
            };
        }
    }
}
