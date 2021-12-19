using System;
using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Queries
{
    public class SearchOrderQuery : IQuery<SearchOrderResponse>
    {
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string CultureName { get; set; }
        public string CustomerId { get; set; }

        public virtual void Map(IResolveConnectionContext<object> context)
        {
            Skip = Convert.ToInt32(context.After ?? 0.ToString());
            Take = context.First ?? context.PageSize ?? 10;
            CultureName = context.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase());
            Filter = context.GetArgument<string>("filter");
            Sort = context.GetArgument<string>("sort");
            CustomerId = context.GetArgumentOrValue<string>("userId");
        }
    }
}
