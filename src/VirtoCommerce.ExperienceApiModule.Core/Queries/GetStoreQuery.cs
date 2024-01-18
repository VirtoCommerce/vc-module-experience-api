using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetStoreQuery : Query<StoreResponse>
    {
        public string StoreId { get; set; }

        public string CultureName { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(CultureName));
        }

        public override void Map(IResolveFieldContext context)
        {
            StoreId = context.GetArgument<string>(nameof(StoreId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
        }
    }
}
