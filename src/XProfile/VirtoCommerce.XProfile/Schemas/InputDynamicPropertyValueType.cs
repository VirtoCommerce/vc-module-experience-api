using GraphQL.Types;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class DynamicPropertyValue : DynamicPropertyObjectValue
    {
        public string Name { get; set; }
    }

    public class InputDynamicPropertyValueType : InputObjectGraphType<DynamicPropertyValue>
    {
        public InputDynamicPropertyValueType()
        {
            Field(x => x.Name).Description("Dynamic property name");
            Field<StringGraphType>(nameof(DynamicPropertyObjectValue.Value), "Dynamic property value. ID must be passed for dictionary item");
            Field(x => x.Locale, true).Description("Language (\"en-US\") for multilingual property");
        }
    }
}
