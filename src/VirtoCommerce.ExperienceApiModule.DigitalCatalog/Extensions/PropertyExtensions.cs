using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties, string cultureName)
        {
            return properties.SelectMany(property =>
            {
                var propertyValues = property.Dictionary
                    // Group by Alias for dictionary properties
                    ? property.Values
                        .GroupBy(propertyValue => propertyValue.Alias)
                        .Select(aliasGroup
                            => aliasGroup.FirstOrDefault(propertyValue => propertyValue.LanguageCode.EqualsInvariant(cultureName))
                            // If localization not found build default value
                            ?? aliasGroup.Select(propertyValue =>
                            {
                                var clonedValue = (PropertyValue)propertyValue.Clone();
                                clonedValue.Value = aliasGroup.Key;
                                return clonedValue;
                            }).First()
                        )
                    : property.Values.Where(x => x.LanguageCode.EqualsInvariant(cultureName));

                return propertyValues
                    .Select(propertyValue => propertyValue.CopyPropertyWithValue(property))
                    .DefaultIfEmpty((Property)property.Clone());
            }).ToList();
        }

        public static Property CopyPropertyWithValue(this PropertyValue propertyValue, Property property)
        {
            var clonedProperty = (Property)property.Clone();
            clonedProperty.Values = new List<PropertyValue> { propertyValue };
            return clonedProperty;
        }

    }
}
