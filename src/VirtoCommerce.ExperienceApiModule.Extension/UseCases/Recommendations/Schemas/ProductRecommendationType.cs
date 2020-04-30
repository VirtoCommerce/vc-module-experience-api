using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Requests;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations;
using VirtoCommerce.ExperienceApiModule.GraphQLEx;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
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
            Field(d => d.Type).Description("The recommendation type.");


            var productField = new FieldType
            {
                Name = "product",
                Type = GraphTypeExtenstionHelper.GetActualType<ProductType>(),
                Resolver = new AsyncFieldResolver<ProductRecommendation, object>(async context =>
                {
                    var includeFields = context.SubFields.Values.GetAllNodesPaths().Select(x => x.TrimStart("items.")).ToArray();
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, CatalogProduct>($"recommendedProducts", (ids) => LoadProductsAsync(mediator, new LoadProductRequest { Ids = ids.ToArray(), IncludeFields = includeFields.ToArray() }));

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.ProductId);
                    return await loadHandle;
                })
            };
            AddField(productField);         

        }

        public static async Task<IDictionary<string, CatalogProduct>> LoadProductsAsync(IMediator mediator, LoadProductRequest request)
        {
            var response = await mediator.Send(request);
            return response.Products.ToDictionary(x => x.Id, x => x);
        }
    }
}
