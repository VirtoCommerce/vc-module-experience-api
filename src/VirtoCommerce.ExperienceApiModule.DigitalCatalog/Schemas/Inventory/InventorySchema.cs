using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries.Inventory;

namespace VirtoCommerce.XDigitalCatalog.Schemas.Inventory;

public class InventorySchema : ISchemaBuilder
{
    private readonly IMediator _mediator;

    public InventorySchema(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Build(ISchema schema)
    {
        var filfillmentCenterField = new FieldType
        {
            Name = "fulfillmentCenter",
            Arguments = new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "ID of the Fulfilment Center" }
            ),
            Type = GraphTypeExtenstionHelper.GetActualType<FulfillmentCenterType>(),
            Resolver = new AsyncFieldResolver<object>(async context =>
            {
                context.CopyArgumentsToUserContext();

                var id = context.GetArgument<string>("id");
                var storeId = context.GetArgument<string>("storeId");

                var request = new GetFulfillmentCenterQuery
                {
                    Id = id,
                    StoreId = storeId,
                };
                var fulfillmentCenter = await _mediator.Send(request);

                if (fulfillmentCenter is not null)
                {
                    context.SetExpandedObjectGraph(fulfillmentCenter);
                }

                return fulfillmentCenter;
            })
        };
        schema.Query.AddField(filfillmentCenterField);

        var fulfillmentCentersConnectionBuilder = GraphTypeExtenstionHelper
            .CreateConnection<FulfillmentCenterType, object>()
            .Name("fulfillmentCenters")
            .PageSize(20)
            .Argument<StringGraphType>("storeId", "Search FFCs attached to a store")
            .Argument<StringGraphType>("query", "Search FFC by name")
            .Argument<StringGraphType>("sort", "The sort expression")
            .Argument<ListGraphType<StringGraphType>>("fulfillmentCenterIds", "Filter by FFC IDs");

        fulfillmentCentersConnectionBuilder.ResolveAsync(async context => await ResolveFulfillmentCentersConnectionAsync(_mediator, context));
        schema.Query.AddField(fulfillmentCentersConnectionBuilder.FieldType);
    }

    private async Task<object> ResolveFulfillmentCentersConnectionAsync(IMediator mediator, IResolveConnectionContext<object> context)
    {
        context.CopyArgumentsToUserContext();

        var take = context.First ?? context.PageSize ?? 20;
        var skip = Convert.ToInt32(context.After ?? 0.ToString());
        var fulfillmentCenterIds = context.GetArgument<List<string>>("fulfillmentCenterIds");

        var query = new SearchFulfillmentCentersQuery();

        if (fulfillmentCenterIds.IsNullOrEmpty())
        {
            query.Skip = skip;
            query.Take = take;
            query.StoreId = context.GetArgument<string>("storeId");
            query.Sort = context.GetArgument<string>("sort");
            query.Query = context.GetArgument<string>("query");
        }
        else
        {
            query.FulfillmentCenterIds = fulfillmentCenterIds.ToArray();
            query.Take = fulfillmentCenterIds.Count;
        }

        var response = await mediator.Send(query);

        return new PagedConnection<FulfillmentCenter>(response.Results, query.Skip, query.Take, response.TotalCount);
    }
}
