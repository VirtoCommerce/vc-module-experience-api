using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Schema;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Schemas;

namespace RecommendationsGatewayModule.Core.Schemas
{
    public class ProductRecommendationType : ObjectGraphType<ProductRecommendation>
    {
        public ProductRecommendationType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader)
        {

            Name = "ProductRecommendation";
            Description = "Product recommendation object";

            Field(d => d.ProductId).Description("The unique ID of the product.");
            Field(d => d.Scenario).Description("The recommendation scenario name.");
            Field(d => d.Score).Description("The recommendation relevance score.");

            
            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<ProductRecommendation, object>(async context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths().Select(x => x.TrimStart("items.")).ToArray();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ExpProduct>($"recommendedProducts", (ids) => LoadProductsAsync(mediator, new LoadProductQuery { Ids = ids.ToArray(), IncludeFields = includeFields.ToArray() }));

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.ProductId);
                    return await loadHandle;
                })
            };
            AddField(productField);         

        }

        public static async Task<IDictionary<string, ExpProduct>> LoadProductsAsync(IMediator mediator, LoadProductQuery request)
        {
            var response = await mediator.Send(request);
            return response.Products.ToDictionary(x => x.Id);
        }
    }
}
