using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;

namespace VirtoCommerce.XDigitalCatalog.Queries;

public class ChildCategoriesQuery : CatalogQueryBase<ChildCategoriesQueryResponse>
{
    public string CategoryId { get; set; }
    public int MaxLevel { get; set; }
    public bool OnlyActive { get; set; }
    public string ProductFilter { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        foreach (var argument in base.GetArguments())
        {
            yield return argument;
        }

        yield return Argument<StringGraphType>(nameof(CategoryId));
        yield return Argument<IntGraphType>(nameof(MaxLevel));
        yield return Argument<BooleanGraphType>(nameof(OnlyActive));
        yield return Argument<StringGraphType>(nameof(ProductFilter));
    }

    public override void Map(IResolveFieldContext context)
    {
        base.Map(context);

        CategoryId = context.GetArgument<string>(nameof(CategoryId));
        MaxLevel = context.GetArgument<int>(nameof(MaxLevel));
        OnlyActive = context.GetArgument<bool>(nameof(OnlyActive));
        ProductFilter = context.GetArgument<string>(nameof(ProductFilter));
    }
}
