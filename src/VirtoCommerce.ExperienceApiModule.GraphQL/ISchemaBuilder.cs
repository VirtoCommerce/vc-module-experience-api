using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{

    public interface ISchemaBuilder
    {
        void Build(ISchema schema);
    }
}
