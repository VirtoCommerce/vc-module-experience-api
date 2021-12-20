using GraphQL;
using GraphQL.Builders;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface IExtendableQuery<T>
    {
        public void Map(T context);
    }
}
