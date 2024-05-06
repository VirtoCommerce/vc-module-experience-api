using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

public abstract class CommandBuilder<TCommand, TResult, TCommandGraphType, TResultGraphType>
    : RequestBuilder<TCommand, TResult, TResultGraphType>
    where TCommand : IRequest<TResult>
    where TCommandGraphType : IInputObjectGraphType
    where TResultGraphType : IGraphType
{
    private const string _argumentName = "command";

    protected CommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    public override void Build(ISchema schema)
    {
        schema.Mutation?.AddField(GetFieldType());
    }

    protected override IEnumerable<QueryArgument> GetArguments()
    {
        var type = GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<TCommandGraphType>>();

        yield return new QueryArgument(type) { Name = _argumentName };
    }

    protected override TCommand GetRequest(IResolveFieldContext<object> context)
    {
        var type = GenericTypeHelper.GetActualType<TCommand>();

        return (TCommand)context.GetArgument(type, _argumentName);
    }
}

public abstract class CommandBuilder<TCommand, TResult, TResultGraphType>
    : RequestBuilder<TCommand, TResult, TResultGraphType>
    where TCommand : IRequest<TResult>
    where TResultGraphType : IGraphType
{
    protected CommandBuilder(IMediator mediator, IAuthorizationService authorizationService)
        : base(mediator, authorizationService)
    {
    }

    public override void Build(ISchema schema)
    {
        schema.Mutation?.AddField(GetFieldType());
    }

    protected override IEnumerable<QueryArgument> GetArguments()
    {
        // no arguments needed for this type of command builder
        return Array.Empty<QueryArgument>();
    }

    protected override TCommand GetRequest(IResolveFieldContext<object> context)
    {
        // just create new instance of command since there's not arguments
        return AbstractTypeFactory<TCommand>.TryCreateInstance();
    }
}
