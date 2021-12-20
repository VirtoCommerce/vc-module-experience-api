using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Extensions
{
    public static class ResolveConnectionContextExtensions
    {
        public static T ExtractQuery<T>(this IResolveConnectionContext<object> context) where T : ISearchOrderQuery
        {
            var query = AbstractTypeFactory<T>.TryCreateInstance();
            query.Map(context);
            return query;
        }
    }

}
