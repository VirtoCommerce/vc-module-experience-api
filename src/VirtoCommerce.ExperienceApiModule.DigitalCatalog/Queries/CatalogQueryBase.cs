using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
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
        public IList<string> IncludeFields { get; set; } = Array.Empty<string>();

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(UserId));
            yield return Argument<StringGraphType>(nameof(CultureName));
            yield return Argument<StringGraphType>(nameof(CurrencyCode));
        }

        public override void Map(IResolveFieldContext context)
        {
            StoreId = context.GetArgument<string>(nameof(StoreId));
            UserId = context.GetArgument<string>(nameof(UserId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
            CurrencyCode = context.GetArgument<string>(nameof(CurrencyCode));
        }
    }
}
