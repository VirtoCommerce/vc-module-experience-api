using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public interface ISchemaFactory
    {
        ISchema GetSchema();
    }
}
