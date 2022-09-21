using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

public abstract class SearchQueryBuilder<TQuery, TResult, TItem, TItemGraphType>
    : QueryBuilder<TQuery, TResult, TItemGraphType>
    where TQuery : IQuery<TResult>, IExtendableQuery, IHasArguments, ISearchQuery
    where TResult : GenericSearchResult<TItem>
    where TItemGraphType : IGraphType
{
    protected virtual int DefaultPageSize => 20;

    protected SearchQueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    protected override FieldType GetFieldType()
    {
        var builder = GraphTypeExtenstionHelper.CreateConnection<TItemGraphType, object>()
            .Name(Name)
            .PageSize(DefaultPageSize);

        ConfigureArguments(builder.FieldType);

        builder.ResolveAsync(async context =>
        {
            var (query, response) = await Resolve(context);
            return new PagedConnection<TItem>(response.Results, query.Skip, query.Take, response.TotalCount);
        });

        return builder.FieldType;
    }
}
