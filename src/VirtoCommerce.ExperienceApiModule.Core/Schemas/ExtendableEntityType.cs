using GraphQL.Types;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class ExtendableEntityType<T> : ExtendableGraphType<T>
        where T : Entity
    {
        public ExtendableEntityType()
        {
            Field<NonNullGraphType<IdGraphType>>(nameof(Entity.Id), "The unique ID of the entity.", resolve: x => x.Source?.Id);
        }
    }
}
