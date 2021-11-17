using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartType : ExtendableGraphType<CartAggregate>
    {
        public CartType(
            ICartAvailMethodsService cartAvailMethods,
            IDynamicPropertyResolverService dynamicPropertyResolverService,
            ICartValidationContextFactory cartValidationContextFactory)
        {
            Field(x => x.Cart.Id, nullable: true).Description("Shopping cart ID");
            Field(x => x.Cart.Name, nullable: false).Description("Shopping cart name");
            Field(x => x.Cart.Status, nullable: true).Description("Shopping cart status");
            Field(x => x.Cart.StoreId, nullable: true).Description("Shopping cart store ID");
            Field(x => x.Cart.ChannelId, nullable: true).Description("Shopping cart channel ID");
            Field<BooleanGraphType>("hasPhysicalProducts",
                "Has physical products",
                resolve: context => AbstractTypeFactory<CartHasPhysicalProductsSpecification>.TryCreateInstance().IsSatisfiedBy(context.Source.Cart));
            Field(x => x.Cart.IsAnonymous, nullable: true).Description("Displays whether the shopping cart is anonymous");
            //PT-5425: Add fields
            //Field(x => x.Customer, nullable: true).Description("Shopping cart user");
            Field(x => x.Cart.CustomerId, nullable: true).Description("Shopping cart user ID");
            Field(x => x.Cart.CustomerName, nullable: true).Description("Shopping cart user name");
            Field(x => x.Cart.OrganizationId, nullable: true).Description("Shopping cart organization ID");
            Field(x => x.Cart.IsRecuring, nullable: true).Description("Displays whether the shopping cart is recurring");
            Field(x => x.Cart.Comment, nullable: true).Description("Shopping cart text comment");

            // Characteristics
            Field(x => x.Cart.VolumetricWeight, nullable: true).Description("Shopping cart volumetric weight value");
            Field(x => x.Cart.WeightUnit, nullable: true).Description("Shopping cart weight unit value");
            Field(x => x.Cart.Weight, nullable: true).Description("Shopping cart weight value");
            //PT-5425: Add fields
            //Field(x => x.MeasureUnit, nullable: true).Description("Shopping cart measurement unit value");
            //Field(x => x.Height, nullable: true).Description("Shopping cart height");
            //Field(x => x.Length, nullable: true).Description("Shopping cart length value");
            //Field(x => x.Width, nullable: true).Description("Shopping cart width value");

            // Money
            Field<MoneyType>("total",
                "Shopping cart total",
                resolve: context => context.Source.Cart.Total.ToMoney(context.Source.Currency));
            Field<MoneyType>("subTotal",
                "Shopping cart subtotal",
                resolve: context => context.Source.Cart.SubTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("subTotalWithTax",
                "Subtotal with tax",
                resolve: context => context.Source.Cart.SubTotalWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("extendedPriceTotal",
                "Total extended price",
                resolve: context => context.Source.LineItems.Sum(i => i.ExtendedPrice).ToMoney(context.Source.Currency));
            Field<MoneyType>("extendedPriceTotalWithTax",
                "Total extended price with tax",
                resolve: context => context.Source.LineItems.Sum(i => i.ExtendedPriceWithTax).ToMoney(context.Source.Currency));
            Field<CurrencyType>("currency",
                "Currency",
                resolve: context => context.Source.Currency);
            Field<MoneyType>("taxTotal",
                "Total tax",
                resolve: context => context.Source.Cart.TaxTotal.ToMoney(context.Source.Currency));
            Field(x => x.Cart.TaxPercentRate, nullable: true).Description("Tax percentage");
            Field(x => x.Cart.TaxType, nullable: true).Description("Shipping tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails",
                "Tax details",
                resolve: context => context.Source.Cart.TaxDetails);
            Field<MoneyType>(nameof(ShoppingCart.Fee).ToCamelCase(), resolve: context => context.Source.Cart.Fee.ToMoney(context.Source.Currency));

            // Shipping
            Field<MoneyType>("shippingPrice",
                "Shipping price",
                resolve: context => context.Source.Cart.ShippingTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("shippingPriceWithTax",
                "Shipping price with tax",
                resolve: context => context.Source.Cart.ShippingTotalWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("shippingTotal",
                "Total shipping",
                resolve: context => context.Source.Cart.ShippingTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("shippingTotalWithTax",
                "Total shipping with tax",
                resolve: context => context.Source.Cart.ShippingTotalWithTax.ToMoney(context.Source.Currency));
            ExtendableField<ListGraphType<ShipmentType>>("shipments",
                "Shipments",
                resolve: context => context.Source.Cart.Shipments);

            FieldAsync<ListGraphType<ShippingMethodType>>("availableShippingMethods", resolve: async context =>
            {
                var rates = await cartAvailMethods.GetAvailableShippingRatesAsync(context.Source);
                //store the pair ShippingMethodType and cart aggregate in the user context for future usage in the ShippingMethodType fields resolvers
                if (rates != null)
                {
                    rates.Apply(x => context.UserContext[x.GetCacheKey()] = context.Source);
                }
                return rates;
            });

            // Payment
            Field<MoneyType>("paymentPrice",
                "Payment price",
                resolve: context => context.Source.Cart.PaymentTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("paymentPriceWithTax",
                "Payment price with tax",
                resolve: context => context.Source.Cart.PaymentTotalWithTax.ToMoney(context.Source.Currency));
            Field<MoneyType>("paymentTotal",
                "Total payment",
                resolve: context => context.Source.Cart.PaymentTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("paymentTotalWithTax",
                "Total payment with tax",
                resolve: context => context.Source.Cart.PaymentTotalWithTax.ToMoney(context.Source.Currency));
            ExtendableField<ListGraphType<PaymentType>>("payments",
                "Payments",
                resolve: context => context.Source.Cart.Payments);
            FieldAsync<ListGraphType<PaymentMethodType>>("availablePaymentMethods",
                "Available payment methods",
                resolve: async context =>
            {
                var methods = await cartAvailMethods.GetAvailablePaymentMethodsAsync(context.Source);
                //store the pair ShippingMethodType and cart aggregate in the user context for future usage in the ShippingMethodType fields resolvers
                if (methods != null)
                {
                    methods.Apply(x => context.UserContext[x.Id] = context.Source);
                }
                return methods;
            });
            //PT-5425: Add fields
            //Field<ListGraphType<PaymentPlanType>>("paymentPlan", resolve: context => context.Source.PaymentPlan);

            //PT-5425: Add fields Extended money
            //Field<MoneyType>("extendedPriceTotal", resolve: context => context.Source.ExtendedPriceTotal);
            //Field<MoneyType>("extendedPriceTotalWithTax", resolve: context => context.Source.ExtendedPriceTotalWithTax);

            // Handling totals
            Field<MoneyType>("handlingTotal",
                "Total handling",
                resolve: context => context.Source.Cart.HandlingTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("handlingTotalWithTax",
                "Total handling with tax",
                resolve: context => context.Source.Cart.HandlingTotalWithTax.ToMoney(context.Source.Currency));

            // Discounts
            Field<MoneyType>("discountTotal",
                "Total discount",
                resolve: context => context.Source.Cart.DiscountTotal.ToMoney(context.Source.Currency));
            Field<MoneyType>("discountTotalWithTax",
                "Total discount with tax",
                resolve: context => context.Source.Cart.DiscountTotalWithTax.ToMoney(context.Source.Currency));
            Field<ListGraphType<DiscountType>>("discounts",
                "Discounts",
                resolve: context => context.Source.Cart.Discounts);

            // Addresses
            ExtendableField<ListGraphType<CartAddressType>>("addresses",
                "Addresses",
                resolve: context => context.Source.Cart.Addresses);

            // Gifts
            FieldAsync<ListGraphType<GiftItemType>>("gifts", "Gifts", resolve: async context =>
            {
                var availableGifts = await cartAvailMethods.GetAvailableGiftsAsync(context.Source);
                return availableGifts.Where(x => x.LineItemId != null);
            });
            FieldAsync<ListGraphType<GiftItemType>>("availableGifts", "Available Gifts", resolve: async context =>
                await cartAvailMethods.GetAvailableGiftsAsync(context.Source)
            );

            // Items
            ExtendableField<ListGraphType<LineItemType>>("items",
                "Items",
                resolve: context => context.Source.LineItems);

            Field<IntGraphType>("itemsCount",
                "Item count",
                resolve: context => context.Source.LineItems.Count());
            Field<IntGraphType>("itemsQuantity",
                "Quantity of items",
                resolve: context => context.Source.LineItems.Sum(x => x.Quantity));
            //PT-5425: Add fields
            //Field<LineItemType>("recentlyAddedItem", resolve: context => context.Source.Cart.RecentlyAddedItem);

            // Coupons
            Field<ListGraphType<CouponType>>("coupons",
                "Coupons",
                resolve: context => context.Source.Coupons);

            // Other
            ExtendableField<ListGraphType<DynamicPropertyValueType>>(
                "dynamicProperties",
                "Cart dynamic property values",
                QueryArgumentPresets.GetArgumentForDynamicProperties(),
                context => dynamicPropertyResolverService.LoadDynamicPropertyValues(context.Source.Cart, context.GetArgumentOrValue<string>("cultureName")));

            FieldAsync<BooleanGraphType>("isValid", "Shows whether the cart is valid",
                QueryArgumentPresets.GetArgumentsForCartValidator(),
                resolve: async context =>
                {
                    var ruleSet = context.GetArgumentOrValue<string>("ruleSet");
                    await EnsureThatCartValidatedAsync(context.Source, cartValidationContextFactory, ruleSet);
                    return context.Source.IsValid;
                },
                deprecationReason: "Deprecated, because of useless (no need to know validation state without details). Use validationErrors field."
            );

            FieldAsync<ListGraphType<ValidationErrorType>>("validationErrors", "A set of errors in case the cart is invalid",
                QueryArgumentPresets.GetArgumentsForCartValidator(),
                resolve: async context =>
                {
                    var ruleSet = context.GetArgumentOrValue<string>("ruleSet");
                    await EnsureThatCartValidatedAsync(context.Source, cartValidationContextFactory, ruleSet);
                    return context.Source.ValidationErrors.OfType<CartValidationError>();
                });
            Field(x => x.Cart.Type, nullable: true).Description("Shopping cart type");
        }

        private async Task EnsureThatCartValidatedAsync(CartAggregate cartAggr, ICartValidationContextFactory cartValidationContextFactory, string ruleSet)
        {
            if (!cartAggr.IsValidated)
            {
                var context = await cartValidationContextFactory.CreateValidationContextAsync(cartAggr);
                //We execute a cart validation only once and by demand, in order to do not introduce  performance issues with fetching data from external services
                //like shipping and tax rates etc.
                await cartAggr.ValidateAsync(context, ruleSet);
            }
        }
    }
}
