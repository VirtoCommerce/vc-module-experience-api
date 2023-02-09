using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadRelatedCatalogOutlineQueryHandler : IQueryHandler<LoadRelatedCatalogOutlineQuery, LoadRelatedCatalogOutlineResponse>
    {
        private readonly ICrudService<Store> _storeService;

        public LoadRelatedCatalogOutlineQueryHandler(IStoreService storeService)
        {
            _storeService = (ICrudService<Store>)storeService;
        }

        public virtual async Task<LoadRelatedCatalogOutlineResponse> Handle(LoadRelatedCatalogOutlineQuery request, CancellationToken cancellationToken)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);
            if (store is null) return new LoadRelatedCatalogOutlineResponse();

            return new LoadRelatedCatalogOutlineResponse
            {
                Outline = request.Outlines.GetOutlinePath(store.Catalog)
            };
        }
    }
}
