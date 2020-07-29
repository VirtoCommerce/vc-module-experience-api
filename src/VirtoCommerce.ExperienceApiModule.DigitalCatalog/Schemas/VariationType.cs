using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Specifications;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<ExpVariation>
    {
        public VariationType()
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

            Field<AvailabilityDataType>(
                "availabilityData",
                resolve: context => new ExpAvailabilityData
                {
                    InventoryAll = context.Source.Inventories,
                    IsBuyable = new CatalogProductIsBuyableSpecification().IsSatisfiedBy(context.Source),
                    IsAvailable = new CatalogProductIsAvailableSpecification().IsSatisfiedBy(context.Source),
                    IsInStock = new CatalogProductIsInStockSpecification().IsSatisfiedBy(context.Source),
                });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.CatalogProduct.Images);

            Field<ListGraphType<PriceType>>("prices", resolve: context => context.Source.ProductPrices);

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties.ConvertToFlatModel());

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.CatalogProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.CatalogProduct.Outlines);
        }
    }
}
