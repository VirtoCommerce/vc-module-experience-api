using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class InputDynamicPropertyValueType : InputObjectGraphType<DynamicPropertyValue>
    {
        public InputDynamicPropertyValueType()
        {
            Field(x => x.Name).Description("Dynamic property name");
            Field<DynamicPropertyValueGraphType>(nameof(DynamicPropertyObjectValue.Value),
                "Dynamic property value. ID must be passed for dictionary item");
            Field<StringGraphType>("locale", resolve: x => x.Source.Locale, description: "Language (\"en-US\") for multilingual property", deprecationReason: "Deprecated. Use cultureName field. Will be removed in v. 1.50+");
            Field("cultureName", x => x.Locale, true).Description("Culture name (\"en-US\") for multilingual property");
        }
    }
}
