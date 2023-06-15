using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchCustomerOrderQuery : SearchOrderQuery
    {
        public string CustomerId { get; set; }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            var connectionContext = (IResolveConnectionContext)context;
            CustomerId = connectionContext.GetArgumentOrValue<string>("userId");
        }
    }
}
