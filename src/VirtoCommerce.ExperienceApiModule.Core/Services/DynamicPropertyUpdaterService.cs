using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public class DynamicPropertyUpdaterService : IDynamicPropertyUpdaterService
    {
        private readonly IDynamicPropertyMetaDataResolver _metadataResolver;
        private readonly IntGraphType _intGraphType = new();
        private readonly DecimalGraphType _decimalGraphType = new();
        private readonly DateTimeGraphType _dateTimeGraphType = new();
        private readonly BooleanGraphType _booleanGraphType = new();

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
            var sourceProperties = new List<DynamicObjectProperty>();
            foreach (var newValuesGroup in values.GroupBy(x => x.Name))
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

                sourceProperties.Add(result);
            }

            var comparer = AnonymousComparer.Create((DynamicObjectProperty x) => x.Name);

            if (entity.DynamicProperties == null)
            {
                entity.DynamicProperties = new List<DynamicObjectProperty>();
            }

            // fill missing values with the original ones. (That enables single value updates)
            sourceProperties = sourceProperties.Union(entity.DynamicProperties, comparer).ToList();

            entity.DynamicProperties = entity.DynamicProperties.ToList();
            sourceProperties.Patch(entity.DynamicProperties, comparer, Patch);
        }

        private object ConvertValue(DynamicPropertyValueType valueType, object value)
        {
            return valueType switch
            {
                DynamicPropertyValueType.Integer => _intGraphType.ParseValue(value),
                DynamicPropertyValueType.Decimal => _decimalGraphType.ParseValue(value),
                DynamicPropertyValueType.Boolean => _booleanGraphType.ParseValue(value),
                DynamicPropertyValueType.DateTime => _dateTimeGraphType.ParseValue(value),
                _ => value is DateTime ? _dateTimeGraphType.Serialize(value) : value,
            };
        }

        private void Patch(DynamicObjectProperty source, DynamicObjectProperty target)
        {
            // override only values of a specific locale for multilingual property. Except dictionary properties.
            if (source.IsMultilingual && !source.IsDictionary)
            {
                var comparer = AnonymousComparer.Create((DynamicPropertyObjectValue x) => x.Locale ?? string.Empty);

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
