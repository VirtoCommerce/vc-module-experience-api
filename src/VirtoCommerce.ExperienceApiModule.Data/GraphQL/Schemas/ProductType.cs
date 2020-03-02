using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Relay.Types;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductType : ObjectGraphType<CatalogProduct>
    {
        private readonly IItemService _productService;
        public ProductType(IItemService productService, IDataLoaderContextAccessor dataLoader)
        {
            _productService = productService;

            Name = "Product";
            Description = "Products are the sellable goods in an e-commerce project on the commercetools platform.";

            Field(d => d.Id).Description("The unique ID of the product.");
            Field(d => d.Name, nullable: false).Description("The name of the product.");

            FieldAsync<ListGraphType<PropertyType>>(
                "properties",
                resolve: async context =>
                {
                    var loader = dataLoader.Context.GetOrAddCollectionBatchLoader<string, Property>("propertyLoader", (ids) => LoadProductsPropertiesAsync(productService, ids));

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.Id);
                    return await loadHandle;
                }
            );          
        }

        public static async Task<ILookup<string, Property>> LoadProductsPropertiesAsync(IItemService productService, IEnumerable<string> ids)
        {
            var products = await productService.GetByIdsAsync(ids.ToArray(), ItemResponseGroup.ItemProperties.ToString());
            return products.SelectMany(x => x.Properties.Select(p => new { ProductId = x.Id, Property = p })).ToLookup(x => x.ProductId, x => x.Property);
        }      
    }
}
