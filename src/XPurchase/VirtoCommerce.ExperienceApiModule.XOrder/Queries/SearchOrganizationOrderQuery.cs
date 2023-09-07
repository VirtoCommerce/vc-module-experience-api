using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrganizationOrderQuery : SearchOrderQuery
    {
        public string OrganizationId { get; set; }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            var connectionContext = (IResolveConnectionContext)context;
            OrganizationId = connectionContext.GetArgumentOrValue<string>("organizationId");
        }
    }
}
