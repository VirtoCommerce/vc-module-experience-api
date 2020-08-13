using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface ISchemaFactory
    {
        ISchema GetSchema();
    }
}
