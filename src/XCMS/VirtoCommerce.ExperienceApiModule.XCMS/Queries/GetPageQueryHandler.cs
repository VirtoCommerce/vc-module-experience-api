using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetPageQueryHandler : IQueryHandler<GetPageQuery, GetPageResponse>
    {
        private readonly IFullTextContentSearchService _searchContentService;

        public GetPageQueryHandler(IFullTextContentSearchService searchContentService)
        {
            _searchContentService = searchContentService;
        }

        public async Task<GetPageResponse> Handle(GetPageQuery request, CancellationToken cancellationToken)
        {
            var criteria = new ContentSearchCriteria
            {
                StoreId = request.StoreId,
                //CultureName = request.CultureName,
                Keyword = request.Keyword
            };
            var result = await _searchContentService.SearchContentAsync(criteria);
            var pages = result.Results.Select(x => new PageItem
            {
                Title = x.Name,
                RelativeUrl = x.RelativeUrl
            });
            return new GetPageResponse { Pages = pages };
        }
    }
}
