using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface ISchemaBuilder
    {
        void Build(ISchema schema);
    }
}
