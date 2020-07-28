using GraphQL.Types;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Specifications;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<ExpVariation>
    {
        public VariationType(IProductInventorySearchService productInventorySearchService)
        {
            Field<StringGraphType>(
                "id",
                description: "Id of variation.",
                resolve: context => context.Source.CatalogProduct.Id
            );

            Field<StringGraphType>(
                "code",
                description: "SKU of variation.",
                resolve: context => context.Source.CatalogProduct.Code
            );

            // TODO: change to connection
            FieldAsync<AvailabilityDataType>("availabilityData", resolve: async context =>
            {
                var product = context.Source.CatalogProduct;

                var invntorySearch = await productInventorySearchService.SearchProductInventoriesAsync(new ProductInventorySearchCriteria
                {
                    ProductId = product.Id,
                });

                return new ExpAvailabilityData
                {
                    InventoryAll = invntorySearch.Results,
                    IsBuyable = new CatalogProductIsBuyableSpecification().IsSatisfiedBy(product),
                    IsAvailable = new CatalogProductIsAvailableSpecification().IsSatisfiedBy(product, invntorySearch.Results),
                    IsInStock = new CatalogProductIsInStockSpecification().IsSatisfiedBy(product, invntorySearch.Results),
                };
            });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.CatalogProduct.Images);

            Field<ListGraphType<PriceType>>("prices", resolve: context => context.Source.ProductPrices);

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties.ConvertToFlatModel());

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.CatalogProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.CatalogProduct.Outlines);
        }
    }
}
