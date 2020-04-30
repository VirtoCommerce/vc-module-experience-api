using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using GraphQL.Types.Relay.DataObjects;
using VirtoCommerce.ExperienceApiModule.CatalogApiClient;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations;
using VirtoCommerce.ExperienceApiModule.GraphQLEx;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
{
    public class ProductRecommendationQuery : ISchemaBuilder
    {
        private readonly IHttpClientFactory _httpClientfactory;
        public ProductRecommendationQuery(IHttpClientFactory httpClientfactory)
        {
            _httpClientfactory = httpClientfactory;
        }
        public void Build(ISchema schema)
        {

            var connectionBuilder = ConnectionBuilderExt.Create<ProductRecommendationType, object>()
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
            var client = _httpClientfactory.CreateClient();
            client.DefaultRequestHeaders.Add("api_key", "0cc0cdf8-73c3-47c0-8f2e-55a5fffd82b5");
            var catalogApiClient = new CatalogModuleProductsClient(@"http://localhost:10645/", client);
            var associationsResponse = await catalogApiClient.SearchProductAssociationsAsync(new ProductAssociationSearchCriteria { Skip = skip, Take = first ?? 20, ObjectIds = new[] { context.GetArgument<string>("itemId") } });

            var result =  new Connection<ProductRecommendation>()
            {
                Edges = associationsResponse.Results.Select((x, index) =>
                        new Edge<ProductRecommendation>()
                        {
                            Cursor = (skip + index).ToString(),
                            Node = new ProductRecommendation { ProductId = x.AssociatedObjectId, Type = x.Type, Score = 0.333m, Scenario = "vc-associations"  },
                        })
                    .ToList(),
                PageInfo = new PageInfo()
                {
                    HasNextPage = associationsResponse.TotalCount > (skip + first),
                    HasPreviousPage = skip > 0,
                    StartCursor = skip.ToString(),
                    EndCursor = Math.Min(associationsResponse.TotalCount, (int)(skip + first)).ToString()
                },
                TotalCount = associationsResponse.TotalCount,
            };

            return result;
        }
    }
}
