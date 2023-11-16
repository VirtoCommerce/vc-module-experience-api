using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;

namespace VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

public abstract class RequestBuilder<TRequest, TResponse, TResponseGraphType> : ISchemaBuilder
    where TRequest : IRequest<TResponse>
    where TResponseGraphType : IGraphType
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;

    protected abstract string Name { get; }

    protected RequestBuilder(
        IMediator mediator,
        IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
        _mediator = mediator;
    }

    public abstract void Build(ISchema schema);

    protected virtual FieldType GetFieldType()
    {
        var builder = FieldBuilder<object, TResponse>
            .Create(GraphTypeExtenstionHelper.GetActualType<TResponseGraphType>())
            .Name(Name)
            .ResolveAsync(async context =>
            {
                var (_, response) = await Resolve(context);
                return response;
            });

        ConfigureArguments(builder.FieldType);

        return builder.FieldType;
    }

    protected virtual void ConfigureArguments(FieldType builder)
    {
        builder.Arguments ??= new QueryArguments();

        foreach (var argument in GetArguments())
        {
            builder.Arguments.Add(argument);
        }
    }

    protected abstract IEnumerable<QueryArgument> GetArguments();

    protected virtual async Task<(TRequest, TResponse)> Resolve(IResolveFieldContext<object> context)
    {
        var request = GetRequest(context);

        await BeforeMediatorSend(context, request);
        var response = await GetResponseAsync(context, request);
        await AfterMediatorSend(context, request, response);

        return (request, response);
    }

    protected abstract TRequest GetRequest(IResolveFieldContext<object> context);

    protected virtual Task BeforeMediatorSend(IResolveFieldContext<object> context, TRequest request)
    {
        return Task.CompletedTask;
    }

    protected virtual Task AfterMediatorSend(IResolveFieldContext<object> context, TRequest request, TResponse response)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task<TResponse> GetResponseAsync(IResolveFieldContext<object> context, TRequest request)
    {
        return await _mediator.Send(request);
    }

    protected virtual async Task Authorize(IResolveFieldContext context, object resource, IAuthorizationRequirement requirement)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), resource, requirement);

        if (!authorizationResult.Succeeded)
        {
            throw context.IsAuthenticated()
                ? AuthorizationError.Forbidden()
                : AuthorizationError.AnonymousAccessDenied();
        }
    }
}
