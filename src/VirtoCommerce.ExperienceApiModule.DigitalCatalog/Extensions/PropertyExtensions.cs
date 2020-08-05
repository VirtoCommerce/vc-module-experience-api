using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ExpandByValues(this IEnumerable<Property> properties)
        {
            var result = properties.SelectMany(property =>
            {
                var propertyValues = property.Values.Select(v =>
                {
                    var result1 = (Property) property.Clone();
                    result1.Values = new List<PropertyValue> { v };
                    return result1;
                }).ToList();

                if (propertyValues.IsNullOrEmpty())
                {
                    propertyValues = new List<Property> { property };
                }

                return propertyValues;
            });

            return result.ToArray();
        }
    }
}
