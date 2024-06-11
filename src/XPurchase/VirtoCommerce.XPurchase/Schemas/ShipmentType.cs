using System.Linq;
using AutoMapper;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ShipmentType : ExtendableGraphType<Shipment>
    {
        public ShipmentType(IMapper mapper,
            IMemberService memberService,
            IDataLoaderContextAccessor dataLoader,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            ICartAvailMethodsService availableMethodsService)
        {
            Field(x => x.Id, nullable: false).Description("Shipment Id");
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
            Field<NonNullGraphType<MoneyType>>("price",
                "Price",
                resolve: context => context.Source.Price.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("priceWithTax",
                "Price with tax",
                resolve: context => context.Source.PriceWithTax.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("fee",
                "Fee",
                resolve: context => context.Source.Fee.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("feeWithTax",
                "Fee with tax",
                resolve: context => context.Source.FeeWithTax.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("total",
                "Total",
                resolve: context => context.Source.Total.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("totalWithTax",
                "Total with tax",
                resolve: context => context.Source.TotalWithTax.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("discountAmount",
                "Discount amount",
                resolve: context => context.Source.DiscountAmount.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<MoneyType>>("discountAmountWithTax",
                "Discount amount with tax",
                resolve: context => context.Source.DiscountAmountWithTax.ToMoney(context.GetCart().Currency));
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<CartShipmentItemType>>>>("items",
                "Items",
                resolve: context => context.Source.Items);
            Field<NonNullGraphType<MoneyType>>("taxTotal",
                "Tax total",
                resolve: context => context.Source.TaxTotal.ToMoney(context.GetCart().Currency));
            Field(x => x.TaxPercentRate, nullable: false).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Tax type");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<TaxDetailType>>>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.TaxDetails);
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<DiscountType>>>>("discounts",
                "Discounts",
                resolve: context => context.Source.Discounts);
            Field<NonNullGraphType<CurrencyType>>("currency",
                "Currency",
                resolve: context => context.GetCart().Currency);
            Field(x => x.Comment, nullable: true).Description("Text comment");

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

            ExtendableField<NonNullGraphType<ListGraphType<NonNullGraphType<DynamicPropertyValueType>>>>(
                "dynamicProperties",
                "Cart shipment dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source, context.GetArgumentOrValue<string>("cultureName")));

            var nameField = new FieldType
            {
                Name = "shippingMethod",
                Type = typeof(ShippingMethodType),
                Resolver = new FuncFieldResolver<Shipment, IDataLoaderResult<ShippingRate>>(context =>
                {
                    var loader = dataLoader.Context.GetOrAddBatchLoader<string, ShippingRate>("cart_shipping_methods", async (codes) =>
                    {
                        var cart = context.GetValueForSource<CartAggregate>();

                        var availableShippingMethods = await availableMethodsService.GetAvailableShippingRatesAsync(cart);

                        return availableShippingMethods
                            .Where(x => x.ShippingMethod != null)
                            .ToDictionary(x => $"{x.ShippingMethod.Code}-{x.OptionName}");
                    });

                    return loader.LoadAsync($"{context.Source.ShipmentMethodCode}-{context.Source.ShipmentMethodOption}");
                })
            };
            AddField(nameField);
        }
    }
}
