using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class LoadPropertiesQueryHandler : IQueryHandler<LoadPropertiesQuery, LoadPropertiesResponse>
    {
        private readonly IPropertyService _propertyService;

        public LoadPropertiesQueryHandler(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        public virtual async Task<LoadPropertiesResponse> Handle(LoadPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _propertyService.GetByIdsAsync(request.Ids);


            return new LoadPropertiesResponse
            {
                Properties = properties.ToDictionary(x => x.Id)
            };
        }
    }
}
