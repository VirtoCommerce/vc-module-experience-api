using GraphQL.Types;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class PropertyTypeEnum : EnumerationGraphType<CatalogModule.Core.Model.PropertyType>
    {
        public PropertyTypeEnum()
        {
            Name = "PropertyType";
            Description = "The type of catalog property.";
        }
    }
}
