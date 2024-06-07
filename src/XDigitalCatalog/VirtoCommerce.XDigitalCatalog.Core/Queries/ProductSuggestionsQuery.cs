using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.XDigitalCatalog.Core.Models;

namespace VirtoCommerce.XDigitalCatalog.Core.Queries;

public class ProductSuggestionsQuery : Query<ProductSuggestionsQueryResponse>
{
    public string StoreId { get; set; }
    public string Query { get; set; }
    public int Size { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<NonNullGraphType<StringGraphType>>(nameof(StoreId));
        yield return Argument<StringGraphType>(nameof(Query));
        yield return Argument<IntGraphType>(nameof(Size));
    }

    public override void Map(IResolveFieldContext context)
    {
        StoreId = context.GetArgument<string>(nameof(StoreId));
        Query = context.GetArgument<string>(nameof(Query));
        Size = context.GetArgument<int>(nameof(Size));
    }
}
