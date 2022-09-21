using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

public abstract class QueryBuilder<TQuery, TResult, TResultGraphType>
    : RequestBuilder<TQuery, TResult, TResultGraphType>
    where TQuery : IQuery<TResult>, IExtendableQuery, IHasArguments
    where TResultGraphType : IGraphType
{
    protected QueryBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    public override void Build(ISchema schema)
    {
        schema.Query.AddField(GetFieldType());
    }

    protected override IEnumerable<QueryArgument> GetArguments()
    {
        return AbstractTypeFactory<TQuery>.TryCreateInstance().GetArguments();
    }

    protected override TQuery GetRequest(IResolveFieldContext<object> context)
    {
        var request = AbstractTypeFactory<TQuery>.TryCreateInstance();
        request.Map(context);

        return request;
    }
}
