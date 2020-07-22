using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<Variation>
    {
        public VariationType(
            IMediator mediator,
            IDataLoaderContextAccessor dataLoader,
            IProductInventorySearchService productInventorySearchService
            )
        {
            Field(x => x.Id, nullable: false).Description("Id of variation.");

            Field(x => x.Code, nullable: true).Description("SKU of variation.");

            FieldAsync<AvailabilityDataType>("availabilityData", resolve: async context =>
            {
                var productId = context.Source.Id;
                var invntorySearch = await productInventorySearchService.SearchProductInventoriesAsync(new ProductInventorySearchCriteria
                {
                    ProductId = productId
                });

                return new ExpAvailabilityData
                {
                    InventoryAll = invntorySearch.Results,
                    IsActive = context.Source.IsActive ?? false,
                    IsBuyable = context.Source.IsBuyable ?? false,
                    TrackInventory = context.Source.TrackInventory ?? false,
                };
            });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Images);

            FieldAsync<ListGraphType<PriceType>>(
                "prices",
                arguments: new QueryArguments
                {
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = Constants.CultureName }
                },
                resolve: async context =>
                {
                    var responce = await mediator.Send(new LoadProductPricesRequest
                    {
                        ProductId = context.Source.Id,
                        Language = context.GetLanguage()
                    });

                    return responce.ProductPrices;
                });

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.Properties);

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.Assets);
        }
    }
}
