using System;
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

        /// <summary>
        /// Updates object's dynamic properties by values collection.
        /// A value object in the collection should have:
        /// Name, this is the name of a dynamic property (DP metadata will be resolved by it);
        /// Property value;
        /// Locale (optional, for a multilanguage property).
        /// </summary>
        /// <param name="entity">Object that has dynamic properties.</param>
        /// <param name="values">Collection on dynamic properties values.</param>
        /// <returns></returns>
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

                    if (!result.IsDictionary)
                    {
                        newValue.Value = ConvertValue(result.ValueType, newValue.Value);
                    }
                    else
                    {
                        // remove locale for Dictonary values, they should not have it
                        newValue.Locale = null;
                    }
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

        private object ConvertValue(DynamicPropertyValueType valueType, object value)
        {
            switch (valueType)
            {
                case DynamicPropertyValueType.ShortText:
                    return (string)value;
                case DynamicPropertyValueType.Decimal:
                    return value.ToNullable<decimal>();
                case DynamicPropertyValueType.DateTime:
                    return value.ToNullable<DateTime>();
                case DynamicPropertyValueType.Boolean:
                    return value.ToNullable<bool>();
                case DynamicPropertyValueType.Integer:
                    return value.ToNullable<int>();
                default:
                    return (string)value;
            }
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
