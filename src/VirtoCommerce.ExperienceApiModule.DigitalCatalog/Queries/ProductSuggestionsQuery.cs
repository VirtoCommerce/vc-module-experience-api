using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ProductSuggestionsQuery : Query<ProductSuggestionsQueryResponse>
{
    public string StoreId { get; set; }
    public string Query { get; set; }
    public IList<string> Fields { get; set; }
    public int Size { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<StringGraphType>(nameof(StoreId));
        yield return Argument<StringGraphType>(nameof(Query));
        yield return Argument<ListGraphType<StringGraphType>>(nameof(Fields));
        yield return Argument<IntGraphType>(nameof(Size));
    }

    public override void Map(IResolveFieldContext context)
    {
        StoreId = context.GetArgument<string>(nameof(StoreId));
        Query = context.GetArgument<string>(nameof(Query));
        Fields = context.GetArgument<IList<string>>(nameof(Fields));
        Size = context.GetArgument<int>(nameof(Size));
    }
}
