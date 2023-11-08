using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class DynamicPropertyValueTypeEnum: EnumerationGraphType<Platform.Core.DynamicProperties.DynamicPropertyValueType>
    {
        public DynamicPropertyValueTypeEnum()
        {
            Name = "DynamicPropertyValueTypes";
            Description = "Dynamic property value type";
        }
    }
}
