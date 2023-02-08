using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadRelatedSlugPathQueryHandler : IQueryHandler<LoadRelatedSlugPathQuery, LoadRelatedSlugPathResponse>
    {
        private readonly ICrudService<Store> _storeService;

        public LoadRelatedSlugPathQueryHandler(ICrudService<Store> storeService)
        {
            _storeService = storeService;
        }

        public virtual async Task<LoadRelatedSlugPathResponse> Handle(LoadRelatedSlugPathQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);
            if (store is null) return new LoadRelatedSlugPathResponse();

            var language = request.CultureName ?? store.DefaultLanguage;
            var slug = request.Outlines.GetSeoPath(store, language, null);

            return new LoadRelatedSlugPathResponse
            {
                Slug = slug
            };
        }
    }
}
