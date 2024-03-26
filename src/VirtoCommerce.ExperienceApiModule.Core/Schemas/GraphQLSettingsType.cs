using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class GraphQLSettingsType : ObjectGraphType<GraphQLSettings>
    {
        public GraphQLSettingsType()
        {
            Field(x => x.KeepAliveInterval, nullable: false).Description("Keep-alive message interval for GraphQL subscription");
        }
    }
}
