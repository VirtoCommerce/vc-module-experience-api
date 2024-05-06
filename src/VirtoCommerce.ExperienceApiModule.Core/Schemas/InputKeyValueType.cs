using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class InputKeyValueType : InputObjectGraphType<KeyValue>
    {
        public InputKeyValueType()
        {
            Field(x => x.Key, nullable: false).Description("Dictionary key");
            Field(x => x.Value, nullable: true).Description("Dictionary value");
        }
    }
}
