using GraphQL;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface IExtendableQuery
    {
        public void Map(IResolveFieldContext context);
    }
}
