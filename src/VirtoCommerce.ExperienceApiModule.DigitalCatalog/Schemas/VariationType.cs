using System.Linq;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Models;
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
                "name",
                description: "Name of variation.",
                resolve: context => context.Source.IndexedProduct.Name
            );

            Field<StringGraphType>(
                "code",
                description: "SKU of variation.",
                resolve: context => context.Source.IndexedProduct.Code
            );

            Field(
                GraphTypeExtenstionHelper.GetActualType<AvailabilityDataType>(),
                "availabilityData",
                resolve: context => new ExpAvailabilityData
                {
                    AvailableQuantity = context.Source.AvailableQuantity,
                    InventoryAll = context.Source.AllInventories,
                    IsBuyable = context.Source.IsBuyable,
                    IsAvailable = context.Source.IsAvailable,
                    IsInStock = context.Source.IsInStock
                });

            Field<ListGraphType<ImageType>>("images", resolve: context => context.Source.IndexedProduct.Images);

            Field<PriceType>("price", resolve: context => context.Source.AllPrices.FirstOrDefault() ?? new ProductPrice(context.GetCurrencyByCode(context.GetValue<string>("currencyCode"))));

            Field<ListGraphType<PriceType>>("prices", resolve: context => context.Source.AllPrices);

            Field(GraphTypeExtenstionHelper.GetActualComplexType<ListGraphType<PropertyType>>(), "properties", resolve: context =>
            {
                var cultureName = context.GetValue<string>("cultureName");
                return context.Source.IndexedProduct.Properties.ExpandByValues(cultureName);
            });

            Field<ListGraphType<AssetType>>("assets", resolve: context => context.Source.IndexedProduct.Assets);

            Field<ListGraphType<OutlineType>>("outlines", resolve: context => context.Source.IndexedProduct.Outlines);
        }
    }
}
