using AutoMapper;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ShipmentType : ExtendableGraphType<Shipment>
    {
        public ShipmentType(IMapper mapper, IMemberService memberService, IDataLoaderContextAccessor dataLoader, IDynamicPropertyResolverService dynamicPropertyResolverService)
        {
            Field(x => x.Id, nullable: true).Description("Shipment Id");
            Field(x => x.ShipmentMethodCode, nullable: true).Description("Shipment method code");
            Field(x => x.ShipmentMethodOption, nullable: true).Description("Shipment method option");
            Field(x => x.FulfillmentCenterId, nullable: true).Description("Fulfillment center id");
            ExtendableField<CartAddressType>("deliveryAddress",
                "Delivery address",
                resolve: context => context.Source.DeliveryAddress);
            Field(x => x.VolumetricWeight, nullable: true).Description("Value of volumetric weight");
            Field(x => x.WeightUnit, nullable: true).Description("Value of weight unit");
            Field(x => x.Weight, nullable: true).Description("Value of weight");
            Field(x => x.MeasureUnit, nullable: true).Description("Value of measurement units");
            Field(x => x.Height, nullable: true).Description("Value of height");
            Field(x => x.Length, nullable: true).Description("Value of length");
            Field(x => x.Width, nullable: true).Description("Value of width");
            Field<MoneyType>("price",
                "Price",
                resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("total",
                "Total",
                resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("totalWithTax",
                "Total with tax",
                resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<MoneyType>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<ListGraphType<CartShipmentItemType>>("items",
                "Items",
                resolve: context => context.Source.Items);
            Field<MoneyType>("taxTotal",
                "Tax total",
                resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
            Field<ListGraphType<DiscountType>>("discounts",
                "Discounts",
                resolve: context => context.Source.Discounts);
            Field<CurrencyType>("currency",
                "Currency",
                resolve: context => context.GetCart().Currency);

            var vendorField = new FieldType
            {
                Name = "vendor",
                Type = GraphTypeExtenstionHelper.GetActualType<VendorType>(),
                Resolver = new FuncFieldResolver<Shipment, IDataLoaderResult<ExpVendor>>(context =>
                {
                    return dataLoader.LoadVendor(memberService, mapper, loaderKey: "cart_vendor", vendorId: context.Source.VendorId);
                })
            };
            AddField(vendorField);

            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Cart shipment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));
        }
    }
}
