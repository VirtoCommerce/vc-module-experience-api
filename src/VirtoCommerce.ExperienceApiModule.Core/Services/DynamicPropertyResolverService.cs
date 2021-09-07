using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public class DynamicPropertyResolverService : IDynamicPropertyResolverService
    {
        private readonly IDynamicPropertySearchService _dynamicPropertySearchService;

        public DynamicPropertyResolverService(IDynamicPropertySearchService dynamicPropertySearchService)
        {
            _dynamicPropertySearchService = dynamicPropertySearchService;
        }

        /// <summary>
        /// Load the dynamic property values for an entity. Include empty meta-data for missing values.
        /// </summary>
        /// <returns>Loaded Dynamic Property Values for specified entity</returns>
        public async Task<IEnumerable<DynamicPropertyObjectValue>> LoadDynamicPropertyValues(IHasDynamicProperties entity, string cultureName)
        {
            // actual values
            var result = entity.DynamicProperties?.SelectMany(x => x.Values) ?? Enumerable.Empty<DynamicPropertyObjectValue>();

            if (!cultureName.IsNullOrEmpty())
            {
                result = result.Where(x => x.Locale.IsNullOrEmpty() || x.Locale.EqualsInvariant(cultureName));
            }

            // find and add all the properties without values
            var criteria = AbstractTypeFactory<DynamicPropertySearchCriteria>.TryCreateInstance();
            criteria.ObjectType = entity.ObjectType;
            criteria.Take = int.MaxValue;
            var searchResult = await _dynamicPropertySearchService.SearchDynamicPropertiesAsync(criteria);

            var propertiesWithoutValue = searchResult.Results.Where(p => entity.DynamicProperties?.All(x => x.Id != p.Id) ?? true);
            var existingDynamicProperties = searchResult.Results.Where(p => entity.DynamicProperties.All(x => x.Id == p.Id || x.Name.EqualsInvariant(p.Name)));
            var propertiesWithoutValue = searchResult.Results.Except(existingDynamicProperties);
            var emptyValues = propertiesWithoutValue.Select(x =>
            {
                var newValue = AbstractTypeFactory<DynamicPropertyObjectValue>.TryCreateInstance();
                newValue.ObjectId = entity.Id;
                newValue.ObjectType = entity.ObjectType;
                newValue.PropertyId = x.Id;
                newValue.PropertyName = x.Name;
                newValue.ValueType = x.ValueType;
                return newValue;
            });

            return result.Union(emptyValues);
        }
    }
}
