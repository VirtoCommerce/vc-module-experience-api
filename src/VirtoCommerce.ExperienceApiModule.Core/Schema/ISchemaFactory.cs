using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schema
{
    public interface ISchemaFactory
    {
        ISchema GetSchema();
    }
}
