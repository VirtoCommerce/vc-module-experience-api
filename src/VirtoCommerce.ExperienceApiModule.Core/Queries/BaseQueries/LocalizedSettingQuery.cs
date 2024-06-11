using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;
using VirtoCommerce.ExperienceApiModule.Core.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries.BaseQueries;

public abstract class LocalizedSettingQuery : Query<LocalizedSettingResponse>
{
    public string CultureName { get; set; }

    public override IEnumerable<QueryArgument> GetArguments()
    {
        yield return Argument<StringGraphType>(nameof(CultureName));
    }

    public override void Map(IResolveFieldContext context)
    {
        CultureName = context.GetArgument<string>(nameof(CultureName));
    }
}
