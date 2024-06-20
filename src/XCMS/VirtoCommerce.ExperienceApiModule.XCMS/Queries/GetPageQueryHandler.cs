using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using static VirtoCommerce.ContentModule.Core.ContentConstants;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries;

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
            ContentType = ContentTypes.Pages,
            LanguageCode = request.CultureName,
            Keyword = request.Keyword,
            Take = request.Take,
            Skip = request.Skip,
        };
        var result = await _searchContentService.SearchAsync(criteria);
        var pages = result.Results.Select(x => new PageItem
        {
            Id = x.Id,
            Name = string.IsNullOrEmpty(x.DisplayName) ? x.Name : x.DisplayName,
            RelativeUrl = x.RelativeUrl,
            Permalink = x.Permalink
        });
        return new GetPageResponse { Pages = pages, TotalCount = result.TotalCount };
    }
}
