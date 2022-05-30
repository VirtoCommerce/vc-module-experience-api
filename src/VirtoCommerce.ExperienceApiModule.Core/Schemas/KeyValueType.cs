using System.Collections.Generic;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class KeyValueType : ObjectGraphType<KeyValuePair<string, string>>
    {
        public KeyValueType()
        {
            Field(x => x.Key, nullable: false).Description("Dictionary key");
            Field(x => x.Value, nullable: true).Description("Dictionary value");
        }
    }
}
