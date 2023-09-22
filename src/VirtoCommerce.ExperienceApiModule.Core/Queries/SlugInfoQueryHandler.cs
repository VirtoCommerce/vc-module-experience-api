using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SlugInfoQueryHandler : IQueryHandler<SlugInfoQuery, SlugInfoResponse>
    {
        private readonly CompositeSeoBySlugResolver _seoBySlugResolver;
        private readonly IStoreService _storeService;

        public SlugInfoQueryHandler(CompositeSeoBySlugResolver seoBySlugResolver, IStoreService storeService)
        {
            _seoBySlugResolver = seoBySlugResolver;
            _storeService = storeService;
        }

        public async Task<SlugInfoResponse> Handle(SlugInfoQuery request, CancellationToken cancellationToken)
        {
            var result = new SlugInfoResponse();

            if (string.IsNullOrEmpty(request.Slug))
            {
                return result;
            }

            var store = await _storeService.GetByIdAsync(request.StoreId);
            if (store is null)
            {
                return result;
            }

            var currentCulture = request.CultureName ?? store.DefaultLanguage;

            var segments = request.Slug.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var lastSegment = segments.Last();
            result.EntityInfo = await GetBestMatchingSeoInfo(store, lastSegment, currentCulture);

            return result;
        }

        private async Task<SeoInfo> GetBestMatchingSeoInfo(Store store, string slug, string currentCulture)
        {
            var seoInfos = await _seoBySlugResolver.FindSeoBySlugAsync(slug);
            var bestMatchSeoInfo = seoInfos.GetBestMatchingSeoInfo(store.Id, currentCulture, slug);
            return bestMatchSeoInfo;
        }
    }
}
