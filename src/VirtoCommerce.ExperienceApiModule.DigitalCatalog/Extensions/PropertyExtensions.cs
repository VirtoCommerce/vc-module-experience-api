using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class PropertyExtensions
    {
        public static IList<Property> ConvertToFlatModel(this IEnumerable<Property> properties)
        {
            return properties
                .SelectMany(property => property.Values
                    .Select(propValue => new Property
                    {
                        Id = property.Id,
                        Name = property.Name,
                        DisplayNames = property.DisplayNames,
                        Hidden = property.Hidden,
                        Multivalue = property.Values.Count > 1,
                        Values = new List<PropertyValue> { propValue }
                    }))
                .ToList();
        }
    }
}
