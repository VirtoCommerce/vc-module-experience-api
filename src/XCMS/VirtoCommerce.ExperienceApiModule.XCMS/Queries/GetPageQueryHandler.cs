using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;
using static VirtoCommerce.ContentModule.Core.ContentConstants;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries;

public class GetPageQueryHandler : IQueryHandler<GetPageQuery, GetPageResponse>
{
    private readonly IFullTextContentSearchService _searchContentService;
    private readonly IStoreService _storeService;

    public GetPageQueryHandler(
        IFullTextContentSearchService searchContentService,
        IStoreService storeService
    )
    {
        _searchContentService = searchContentService;
        _storeService = storeService;
    }

    public async Task<GetPageResponse> Handle(GetPageQuery request, CancellationToken cancellationToken)
    {
        var criteria = new ContentSearchCriteria
        {
            StoreId = request.StoreId,
            ContentType = ContentTypes.Pages,
            Keyword = request.Keyword,
            Take = request.Take,
            Skip = request.Skip,
        };
        var store = await _storeService.GetByIdAsync(request.StoreId);
        var defaultLanguage = store.DefaultLanguage;
        var result = await _searchContentService.SearchContentAsync(criteria);
        var pages = result.Results.Where(x => x.Language == request.CultureName || x.Language == defaultLanguage || x.Language == null)
            .GroupBy(x => x.Permalink)
            .Select(x => x.Count() == 1
                ? x.First()
                : x.FirstOrDefault(_ => _.Language == defaultLanguage)
                  ?? x.First(_ => _.Language == request.CultureName))
            .Select(x => new PageItem
            {
                Id = x.Id,
                Name = string.IsNullOrEmpty(x.DisplayName) ? x.Name : x.DisplayName,
                RelativeUrl = x.RelativeUrl,
                Permalink = x.Permalink
            });
        return new GetPageResponse { Pages = pages, TotalCount = result.TotalCount };
    }
}
