using GraphQL.Builders;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface ISearchOrderQuery
    {
        public void Map(IResolveConnectionContext<object> context);
    }
}
