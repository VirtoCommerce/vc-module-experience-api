using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class CatalogQueryBase<TResponse> : Query<TResponse>, ICatalogQuery
    {
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }

        public Store Store { get; set; }
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId), description: "Store Id");
            yield return Argument<StringGraphType>(nameof(UserId), description: "User Id");
            yield return Argument<StringGraphType>(nameof(CultureName), description: "Currency code (\"USD\")");
            yield return Argument<StringGraphType>(nameof(CurrencyCode), description: "Culture name (\"en-US\")");
        }

        public override void Map(IResolveFieldContext context)
        {
            StoreId = context.GetArgument<string>(nameof(StoreId));
            UserId = context.GetArgument<string>(nameof(UserId)) ?? context.GetCurrentUserId();
            CultureName = context.GetArgument<string>(nameof(CultureName));
            CurrencyCode = context.GetArgument<string>(nameof(CurrencyCode));
        }
    }
}
