using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class SearchVideoQueryHandler : IRequestHandler<SearchVideoQuery, SearchVideoQueryResponse>
    {
        private readonly ISearchService<VideoSearchCriteria, VideoSearchResult, Video> _videoSearchService;

        public SearchVideoQueryHandler(IVideoSearchService videoSearchService)
        {
            _videoSearchService = (ISearchService<VideoSearchCriteria, VideoSearchResult, Video>)videoSearchService;
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
