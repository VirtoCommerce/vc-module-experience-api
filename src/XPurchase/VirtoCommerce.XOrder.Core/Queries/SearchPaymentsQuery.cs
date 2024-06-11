using System;
using GraphQL;
using GraphQL.Builders;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.OrdersModule.Core.Model.Search;

namespace VirtoCommerce.XOrder.Core.Queries
{
    public class SearchPaymentsQuery : IQuery<PaymentSearchResult>, IExtendableQuery
    {
        public string Sort { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string CultureName { get; set; }
        public string CustomerId { get; set; }

        public virtual void Map(IResolveFieldContext context)
        {
            var connectionContext = (IResolveConnectionContext)context;
            Skip = Convert.ToInt32(connectionContext.After ?? 0.ToString());
            Take = connectionContext.First ?? connectionContext.PageSize ?? 10;
            CultureName = connectionContext.GetArgument<string>(nameof(Currency.CultureName).ToCamelCase());
            Filter = connectionContext.GetArgument<string>("filter");
            Sort = connectionContext.GetArgument<string>("sort");
            CustomerId = connectionContext.GetArgumentOrValue<string>("userId");
        }
    }
}
