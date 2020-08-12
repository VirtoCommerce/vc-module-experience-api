using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties, string cultureName)
            => properties.SelectMany(property => property.Values
                .GroupBy(propertyValue => propertyValue.Alias)
                .SelectMany(aliasGroup => cultureName switch
                {
                    string languageCode when languageCode != null
                                        && aliasGroup.Any(x => x.LanguageCode.EqualsInvariant(languageCode))
                        => aliasGroup.Where(x => x.LanguageCode.EqualsInvariant(languageCode)),

                    string languageCode when languageCode != null
                        => aliasGroup
                            .Take(1)
                            .Select(propertyValue =>
                            {
                                var clonedValue = (PropertyValue)propertyValue.Clone();
                                clonedValue.Value = aliasGroup.Key;
                                return clonedValue;
                            }),

                    _ => aliasGroup.AsEnumerable()
                })
                .Select(propertyValue =>
                {
                    var clonedProperty = (Property)property.Clone();
                    clonedProperty.Values = new List<PropertyValue> { propertyValue };
                    return clonedProperty;
                })
                .DefaultIfEmpty((Property)property.Clone())
            ).ToList();
    }
}
