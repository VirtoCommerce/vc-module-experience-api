using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<ExpVariation>
    {
        public VariationType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader,
            IProductInventorySearchService productInventorySearchService
            )
        {
            Field<StringGraphType>(
                "id",
                description: "Id of variation.",
                resolve: context => context.Source.Product.Id
            );

            Field<StringGraphType>(
                "code",
                description: "SKU of variation.",
                resolve: context => context.Source.Product.Code
            );

            // TODO: change to connection
            FieldAsync<AvailabilityDataType>("availabilityData", resolve: async context =>
            {
                var product = context.Source.Product;

                var invntorySearch = await productInventorySearchService.SearchProductInventoriesAsync(new ProductInventorySearchCriteria
                {
                    ProductId = product.Id
                });

                return new ExpAvailabilityData
                {
                    InventoryAll = invntorySearch.Results,
                    IsActive = product.IsActive ?? false,
                    IsBuyable = product.IsBuyable ?? false,
                    TrackInventory = product.TrackInventory ?? false,
                };
            });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Product.Images);

            Field<ListGraphType<PriceType>>("prices", resolve: context => context.Source.Prices);

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.Product.Properties);

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.Product.Assets);
        }
    }
}
