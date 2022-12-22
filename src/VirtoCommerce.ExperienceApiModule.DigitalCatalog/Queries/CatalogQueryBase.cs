using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Queries
{
    public class CatalogQueryBase<TResponse> : Query<TResponse>, ICatalogQuery
    {
        public IEnumerable<string> IncludeFields { get; set; } = Array.Empty<string>();
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }
        public string CurrencyCode { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(UserId));
            yield return Argument<StringGraphType>(nameof(CultureName));
            yield return Argument<StringGraphType>(nameof(CurrencyCode));
        }

        public override void Map(IResolveFieldContext context)
        {
            IncludeFields = context.SubFields?.Values.GetAllNodesPaths().ToArray() ?? Array.Empty<string>();
            StoreId = context.GetArgument<string>(nameof(StoreId));
            UserId = context.GetArgument<string>(nameof(UserId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
            CurrencyCode = context.GetArgument<string>(nameof(CurrencyCode));
        }
    }
}
