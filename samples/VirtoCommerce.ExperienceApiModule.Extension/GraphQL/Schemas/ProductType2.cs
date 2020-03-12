using GraphQL.DataLoader;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas
{
    public class ProductType2 : ProductType
    {
        public ProductType2(
            IItemService productService,
            IProductAssociationSearchService associationSearchService,
            IDataLoaderContextAccessor dataLoader)
            : base(productService, associationSearchService, dataLoader)
        {
            Field(d => d.OuterId).Description("the product outerId");
        }

    }
}
