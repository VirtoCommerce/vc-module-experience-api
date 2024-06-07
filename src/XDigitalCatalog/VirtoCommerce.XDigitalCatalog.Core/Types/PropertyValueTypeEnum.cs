using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class PropertyValueTypeEnum : EnumerationGraphType<PropertyValueType>
    {
        public PropertyValueTypeEnum()
        {
            Name = "PropertyValueTypes";
            Description = "The type of catalog property value.";
        }
    }
}
