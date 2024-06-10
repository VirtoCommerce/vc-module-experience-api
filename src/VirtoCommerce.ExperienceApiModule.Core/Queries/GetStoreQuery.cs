using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class GetStoreQuery : Query<StoreResponse>
    {
        public string StoreId { get; set; }

        public string CultureName { get; set; }

        public string Domain { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            yield return Argument<StringGraphType>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(CultureName));
            yield return Argument<StringGraphType>(nameof(Domain), "The domain name of the web host");
        }

        public override void Map(IResolveFieldContext context)
        {
            StoreId = context.GetArgument<string>(nameof(StoreId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
            Domain = context.GetArgument<string>(nameof(Domain));
        }
    }
}
