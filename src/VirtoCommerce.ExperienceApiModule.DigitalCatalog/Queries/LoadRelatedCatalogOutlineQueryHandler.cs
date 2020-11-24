using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadRelatedCatalogOutlineQueryHandler : IQueryHandler<LoadRelatedCatalogOutlineQuery, LoadRelatedCatalogOutlineResponse>
    {
        private readonly IStoreService _storeService;

        public LoadRelatedCatalogOutlineQueryHandler(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<LoadRelatedCatalogOutlineResponse> Handle(LoadRelatedCatalogOutlineQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);
            if (store is null) return null;

            return new LoadRelatedCatalogOutlineResponse
            {
                Outline = request.Outlines.GetOutlinePath(store.Catalog)
            };
        }
    }

    public class LoadRelatedSlugPathQueryHandler : IQueryHandler<LoadRelatedSlugPathQuery, LoadRelatedSlugPathResponse>
    {
        private readonly IStoreService _storeService;

        public LoadRelatedSlugPathQueryHandler(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<LoadRelatedSlugPathResponse> Handle(LoadRelatedSlugPathQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);
            if (store is null) return null;

            var language = request.CultureName ?? store.DefaultLanguage;
            var slug = request.Outlines.GetSeoPath(store, language, null);

            return new LoadRelatedSlugPathResponse
            {
                Slug = slug
            };
        }
    }
}
