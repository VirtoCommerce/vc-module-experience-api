using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
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
