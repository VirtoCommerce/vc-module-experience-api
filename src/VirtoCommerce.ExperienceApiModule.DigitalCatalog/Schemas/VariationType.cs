using System.Linq;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ExtendableGraphType<ExpVariation>
    {
        public VariationType()
        {
            Field<StringGraphType>(
                "id",
                description: "Id of variation.",
                resolve: context => context.Source.IndexedProduct.Id
            );

            Field<StringGraphType>(
                "name",
                description: "Name of variation.",
                resolve: context => context.Source.IndexedProduct.Name
            );

            Field<StringGraphType>(
                "code",
                description: "SKU of variation.",
                resolve: context => context.Source.IndexedProduct.Code
            );

            Field<IntGraphType>(
                "minQuantity",
                description: "Min. quantity.",
                resolve: context => context.Source.IndexedProduct.MinQuantity
            );

            Field<IntGraphType>(
                "maxQuantity",
                description: "Max. quantity.",
                resolve: context => context.Source.IndexedProduct.MaxQuantity
            );

            ExtendableField<AvailabilityDataType>(
                "availabilityData",
                "Availability data",
                resolve: context => new ExpAvailabilityData
                {
                    AvailableQuantity = context.Source.AvailableQuantity,
                    InventoryAll = context.Source.AllInventories,
                    IsBuyable = context.Source.IsBuyable,
                    IsAvailable = context.Source.IsAvailable,
                    IsInStock = context.Source.IsInStock
                });

            Field<ListGraphType<ImageType>>("images",
                "Product images",
                resolve: context => context.Source.IndexedProduct.Images);

            Field<PriceType>("price",
                "Product price",
                resolve: context => context.Source.AllPrices.FirstOrDefault() ?? new ProductPrice(context.GetCurrencyByCode(context.GetValue<string>("currencyCode"))));

            Field<ListGraphType<PriceType>>("prices",
                "Product prices",
                resolve: context => context.Source.AllPrices);

            ExtendableField<ListGraphType<PropertyType>>("properties", resolve: context =>
            {
                var cultureName = context.GetValue<string>("cultureName");
                return context.Source.IndexedProduct.Properties.ExpandByValues(cultureName);
            });

            Field<ListGraphType<AssetType>>("assets",
                "Assets",
                resolve: context => context.Source.IndexedProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines",
                "Outlines",
                resolve: context => context.Source.IndexedProduct.Outlines);
        }
    }
}
