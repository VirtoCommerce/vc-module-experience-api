using GraphQL.DataLoader;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
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
                var invntorySearch = await productInventorySearchService.SearchProductInventoriesAsync(new InventoryModule.Core.Model.Search.ProductInventorySearchCriteria
                {
                    ProductId = productId
                });

                return new ExpAvailabilityData
                {
                    InventoryAll = invntorySearch.Results
                };
            });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.Images);

            FieldAsync<ListGraphType<PriceType>>(
                "prices",
                arguments: new QueryArguments
                {
                    new QueryArgument<StringGraphType> { Name = Constants.CultureName }
                },
                resolve: async context =>
                {
                    var responce = await mediator.Send(new LoadProductPricesRequest
                    {
                        ProductId = context.Source.Id,
                        Language = context.GetLanguage(nullable: false)
                    });

                    return responce.ProductPrices;
                });

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.Properties);

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.Assets);
        }
    }
}
