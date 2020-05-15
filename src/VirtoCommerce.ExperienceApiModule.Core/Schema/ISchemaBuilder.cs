using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schema
{
    public interface ISchemaBuilder
    {
        void Build(ISchema schema);
    }
}
