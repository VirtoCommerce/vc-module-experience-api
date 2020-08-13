using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using MediatR;
using RecommendationsGatewayModule.Core.Requests;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace RecommendationsGatewayModule.Core.Schemas
{
    public class ProductRecommendationSchema : ISchemaBuilder
    {
        private readonly IMediator _mediator;
        public ProductRecommendationSchema(IMediator mediator)
        {
            _mediator = mediator;
        }
        public void Build(ISchema schema)
        {

            var connectionBuilder = GraphTypeExtenstionHelper.CreateConnection<ProductRecommendationType, object>()
                .Name("recommendations")
                .Argument<StringGraphType>("scenario", "The recommendation scenario")
                .Argument<StringGraphType>("itemId", "The context product id")
                .Argument<StringGraphType>("userId", "The context user id")
                .Unidirectional()
                .PageSize(20);

            connectionBuilder.ResolveAsync(async context =>
            {
                return await ResolveConnectionAsync(context);
            });
            schema.Query.AddField(connectionBuilder.FieldType);
        }

        private async Task<object> ResolveConnectionAsync(IResolveConnectionContext<object> context)
        {
            var first = context.First;
            var skip = Convert.ToInt32(context.After ?? 0.ToString());

            var response = await _mediator.Send(new GetRecommendationsRequest
            {
                Skip = skip,
                Take = first ?? 20,
                Scenario = context.GetArgument<string>("scenario"),
                ItemId = context.GetArgument<string>("itemId"),
                UserId = context.GetArgument<string>("userId")
            });

            var result =  new Connection<ProductRecommendation>()
            {
                Edges = response.Products.Select((x, index) =>
                        new Edge<ProductRecommendation>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = x,
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = response.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(response.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = response.TotalCount,
            };

            return result;
        }
    }
}
