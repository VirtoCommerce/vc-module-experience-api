using System.Collections.Generic;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

public class SearchQuery<TResult> : Query<TResult>, ISearchQuery
{
    public string Keyword { get; set; }
    public string Sort { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<StringGraphType>(nameof(Keyword));
        yield return Argument<StringGraphType>(nameof(Sort));
    }

    public override void Map(IResolveFieldContext context)
    {
        Keyword = context.GetArgument<string>(nameof(Keyword));
        Sort = context.GetArgument<string>(nameof(Sort));

        if (context is IResolveConnectionContext connectionContext)
        {
            Skip = int.TryParse(connectionContext.After, out var skip) ? skip : 0;
            Take = connectionContext.First ?? connectionContext.PageSize ?? 20;
        }
    }

    public TCriteria GetSearchCriteria<TCriteria>()
        where TCriteria : SearchCriteriaBase
    {
        var criteria = AbstractTypeFactory<TCriteria>.TryCreateInstance();

        criteria.Keyword = Keyword;
        criteria.Sort = Sort;
        criteria.Skip = Skip;
        criteria.Take = Take;

        return criteria;
    }
}
