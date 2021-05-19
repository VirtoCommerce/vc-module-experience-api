using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
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
