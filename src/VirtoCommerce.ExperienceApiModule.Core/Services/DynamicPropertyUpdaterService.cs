using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public class DynamicPropertyUpdaterService : IDynamicPropertyUpdaterService
    {
        private readonly IDynamicPropertyMetaDataResolver _metadataResolver;

        public DynamicPropertyUpdaterService(IDynamicPropertyMetaDataResolver metadataResolver)
        {
            _metadataResolver = metadataResolver;
        }

        public virtual async Task UpdateDynamicPropertyValues(IHasDynamicProperties entity, IList<DynamicPropertyValue> values)
        {
            var tasks = values.GroupBy(x => x.Name).Select(async newValuesGroup =>
            {
                var result = AbstractTypeFactory<DynamicObjectProperty>.TryCreateInstance();
                result.Name = newValuesGroup.Key;

                var metadata = await _metadataResolver.GetByNameAsync(entity.ObjectType, newValuesGroup.Key);
                if (metadata != null)
                {
                    result.SetMetaData(metadata);
                }

                if (result.IsDictionary)
                {
                    // all Values actually are IDs of dictionary values => set ValueIds
                    foreach (var propValue in newValuesGroup.Where(x => x.Value is string))
                    {
                        propValue.ValueId = (string)propValue.Value;
                    }
                }

                foreach (var newValue in newValuesGroup)
                {
                    newValue.PropertyId = result.Id;
                    newValue.PropertyName = result.Name;
                    newValue.ValueType = result.ValueType;
                }

                result.Values = newValuesGroup.ToArray();
                return result;
            });

            var sourceProperties = await Task.WhenAll(tasks);

            var comparer = AnonymousComparer.Create((DynamicObjectProperty x) => x.Name);

            // fill missing values with the original ones. (That enables single value updates)
            sourceProperties = sourceProperties.Union(entity.DynamicProperties, comparer).ToArray();

            entity.DynamicProperties = entity.DynamicProperties.ToList();
            sourceProperties.Patch(entity.DynamicProperties, comparer, Patch);
        }

        private void Patch(DynamicObjectProperty source, DynamicObjectProperty target)
        {
            // override only values of a specific locale for multilingual property. Except dictionary properties.
            if (source.IsMultilingual && !source.IsDictionary)
            {
                var comparer = AnonymousComparer.Create((DynamicPropertyObjectValue x) => x.Locale);

                // fill missing values with the original ones. (That enables single value updates)
                source.Values = source.Values.Union(target.Values, comparer).ToArray();

                target.Values = target.Values.ToList();
                source.Values.Patch(target.Values, comparer, Patch);
            }
            else
            {
                target.Values = source.Values;
            }
        }

        private void Patch(DynamicPropertyObjectValue source, DynamicPropertyObjectValue target)
        {
            target.Value = source.Value;
        }
    }
}
