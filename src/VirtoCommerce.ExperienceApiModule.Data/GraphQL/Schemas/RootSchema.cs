using GraphQL.Types;
using GraphQL.Utilities;
using System;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class RootSchema : Schema
    {
        public RootSchema(IServiceProvider provider)
            : base(provider)
        {
            Query = provider.GetRequiredService<RootQuery>();
        }
    }
}
