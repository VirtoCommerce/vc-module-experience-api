using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties)
        {
            return properties
                .SelectMany(property => property.Values
                    .Select(propValue =>
                    {
                        var result = (Property) property.Clone();
                        result.Values = new List<PropertyValue> { propValue };
                        return result;
                    }))
                .ToList();
        }
    }
}
