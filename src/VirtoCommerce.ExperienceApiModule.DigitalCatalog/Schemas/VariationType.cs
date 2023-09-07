using System.Linq;
using GraphQL.Types;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class VariationType : ExtendableGraphType<ExpVariation>
    {
        public VariationType(IMediator mediator)
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
                resolve: context => AbstractTypeFactory<ExpAvailabilityData>.TryCreateInstance().FromProduct(context.Source));

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

            FieldAsync<StringGraphType>("slug", resolve: async context =>
            {
                var outlines = context.Source.IndexedProduct.Outlines;
                if (outlines.IsNullOrEmpty()) return null;

                var loadRelatedSlugPathQuery = context.GetCatalogQuery<LoadRelatedSlugPathQuery>();
                loadRelatedSlugPathQuery.Outlines = outlines;

                var response = await mediator.Send(loadRelatedSlugPathQuery);
                return response.Slug;
            }, description: "Request related slug for product");

            Field(
                GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                "vendor",
                "Product vendor",
                resolve: context => context.Source.Vendor);
        }
    }
}
