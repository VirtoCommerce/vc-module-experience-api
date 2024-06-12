using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.XCMS.Core.Models;
using VirtoCommerce.XCMS.Core.Queries;
using static VirtoCommerce.ContentModule.Core.ContentConstants;

namespace VirtoCommerce.XCMS.Data.Queries;

public class GetSinglePageQueryHandler : IQueryHandler<GetSinglePageQuery, PageItem>
{
    private readonly IFullTextContentSearchService _searchContentService;

    public GetSinglePageQueryHandler(IFullTextContentSearchService searchContentService)
    {
        _searchContentService = searchContentService;
    }

    public async Task<PageItem> Handle(GetSinglePageQuery request, CancellationToken cancellationToken)
    {
        var criteria = new ContentSearchCriteria
        {
            StoreId = request.StoreId,
            ContentType = ContentTypes.Pages,
            ObjectIds = [request.Id],
            Take = 1,
            Skip = 0,
        };
        var result = await _searchContentService.SearchContentAsync(criteria);

        var page = result.Results.Select(x => new PageItem
        {
            Id = x.Id,
            Name = string.IsNullOrEmpty(x.DisplayName) ? x.Name : x.DisplayName,
            RelativeUrl = x.RelativeUrl,
            Permalink = x.Permalink
        }).First();

        return page;
    }
}
