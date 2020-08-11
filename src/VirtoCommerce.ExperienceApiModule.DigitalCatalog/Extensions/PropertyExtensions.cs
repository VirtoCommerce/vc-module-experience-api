using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties, string cultureName)
        {
            var result = properties.SelectMany(property =>
            {
                var propertyValues = property.Values
                    // Filter properties if cultureName passed
                    .Where(propertyValue => cultureName == null || propertyValue.LanguageCode.EqualsInvariant(cultureName))
                    .Select(propertyValue =>
                    {
                        var clonedProperty = (Property)property.Clone();
                        clonedProperty.Values = new List<PropertyValue> { propertyValue };
                        return clonedProperty;
                    }).ToList();

                return propertyValues.IsNullOrEmpty()
                    ? new List<Property> { (Property)property.Clone() }
                    : propertyValues;
            });

            return result.ToList();
        }
    }
}
