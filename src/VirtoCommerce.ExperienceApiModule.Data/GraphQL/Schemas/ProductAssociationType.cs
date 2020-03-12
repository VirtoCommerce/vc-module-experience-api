using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Types;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductAssociationType : ObjectGraphType<ProductAssociation>
    {
        public ProductAssociationType(IDataLoaderContextAccessor dataLoader, IItemService productService)
        {
            Name = "ProductAssociation";
            Description = "product association.";

            Field(d => d.Type);
            Field(d => d.Priority);
            Field("Quantity", x => x.Quantity, nullable: true, type: typeof(IntGraphType));
            Field(d => d.AssociatedObjectId);
            Field(d => d.AssociatedObjectType);

            FieldAsync<ProductType>(
               "product",          
               resolve: async context =>
               {
                   var loader = dataLoader.Context.GetOrAddBatchLoader<string, CatalogProduct>("associatedProductLoader", (ids) => LoadProductsAsync(productService, ids));

                    // IMPORTANT: In order to avoid deadlocking on the loader we use the following construct (next 2 lines):
                    var loadHandle = loader.LoadAsync(context.Source.AssociatedObjectId);
                   return await loadHandle;
               }
           );
        }

        public static async Task<IDictionary<string, CatalogProduct>> LoadProductsAsync(IItemService productService, IEnumerable<string> ids)
        {
            var products = await productService.GetByIdsAsync(ids.ToArray(), ItemResponseGroup.ItemInfo.ToString());
            return products.ToDictionary(x => x.Id);
        }
    }
}
