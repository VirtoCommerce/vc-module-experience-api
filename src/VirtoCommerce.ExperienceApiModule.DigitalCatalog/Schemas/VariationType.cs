using GraphQL.Types;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ObjectGraphType<ExpVariation>
    {
        public VariationType()
        {
            Field<StringGraphType>(
                "id",
                description: "Id of variation.",
                resolve: context => context.Source.IndexedProduct.Id
            );

            Field<StringGraphType>(
                "code",
                description: "SKU of variation.",
                resolve: context => context.Source.IndexedProduct.Code
            );

            Field<AvailabilityDataType>(
                "availabilityData",
                resolve: context => new ExpAvailabilityData
                {
                    InventoryAll = context.Source.AllInventories,
                    IsBuyable = context.Source.IsBuyable,
                    IsAvailable = context.Source.IsAvailable,
                    IsInStock = context.Source.IsInStock
                });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.IndexedProduct.Images);

            Field<ListGraphType<PriceType>>("prices", resolve: context => context.Source.AllPrices);

            Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.IndexedProduct.Properties.ConvertToFlatModel());

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.IndexedProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.IndexedProduct.Outlines);
        }
    }
}
