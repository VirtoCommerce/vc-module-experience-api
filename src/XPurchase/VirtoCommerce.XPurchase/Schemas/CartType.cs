using System.Linq;
using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Converters;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Extensions;
using VirtoCommerce.XPurchase.Models.Marketing.Services;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class CartType : ObjectGraphType<ShoppingCart>
    {
        public CartType(IPromotionEvaluator promotionEvaluator,
            ITaxEvaluator taxEvaluator,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IShippingMethodsSearchService shippingMethodsSearchService)
        {
            Field(x => x.Name, nullable: false).Description("Shopping cart name");
            Field(x => x.Status, nullable: true).Description("Shopping cart status");
            Field(x => x.StoreId, nullable: true).Description("Shopping cart store id");
            Field(x => x.ChannelId, nullable: true).Description("Shopping cart channel id");
            Field(x => x.HasPhysicalProducts, nullable: true).Description("Sign that shopping cart contains line items which require shipping");
            Field(x => x.IsAnonymous, nullable: true).Description("Sign that shopping cart is anonymous");
            //Field(x => x.Customer, nullable: true).Description("Shopping cart user"); //todo: add resolver
            Field(x => x.CustomerId, nullable: true).Description("Shopping cart user id");
            Field(x => x.CustomerName, nullable: true).Description("Shopping cart user name");
            Field(x => x.OrganizationId, nullable: true).Description("Shopping cart organization id");
            Field(x => x.IsRecuring, nullable: true).Description("Sign that shopping cart is recurring");
            Field(x => x.Comment, nullable: true).Description("Shopping cart text comment");

            // Characteristics
            Field(x => x.VolumetricWeight, nullable: true).Description("Shopping cart value of volumetric weight");
            Field(x => x.WeightUnit, nullable: true).Description("Shopping cart value of weight unit");
            Field(x => x.Weight, nullable: true).Description("Shopping cart value of shopping cart weight");
            Field(x => x.MeasureUnit, nullable: true).Description("Shopping cart value of measurement unit");
            Field(x => x.Height, nullable: true).Description("Shopping cart value of height");
            Field(x => x.Length, nullable: true).Description("Shopping cart value of length");
            Field(x => x.Width, nullable: true).Description("Shopping cart value of width");

            // Money
            Field<MoneyType>("total", resolve: context => context.Source.Total);
            Field<MoneyType>("subTotal", resolve: context => context.Source.SubTotal);
            Field<MoneyType>("subTotalWithTax", resolve: context => context.Source.SubTotalWithTax);
            Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Shipping tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);

            // Shipping
            Field<MoneyType>("shippingPrice", resolve: context => context.Source.ShippingPrice);
            Field<MoneyType>("shippingPriceWithTax", resolve: context => context.Source.ShippingPriceWithTax);
            Field<MoneyType>("shippingTotal", resolve: context => context.Source.ShippingTotal);
            Field<MoneyType>("shippingTotalWithTax", resolve: context => context.Source.ShippingTotalWithTax);
            Field<ListGraphType<ShipmentType>>("shipments", resolve: context => context.Source.Shipments);
            FieldAsync<ListGraphType<ShippingMethodType>>("availableShippingMethods", resolve: async context =>
            {
                var shoppingCart = context.Source;

                if (string.IsNullOrEmpty(shoppingCart.StoreId))
                {
                    return Enumerable.Empty<ShippingMethod>();
                }

                var criteria = new ShippingMethodsSearchCriteria
                {
                    IsActive = true,
                    Take = int.MaxValue,
                    StoreId = shoppingCart.StoreId,
                };

                var Shippings = await shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);

                var result = Shippings.Results.Select(x => x.ToShippingMethod(shoppingCart.Currency)).OrderBy(x => x.Priority).ToList();

                if (shoppingCart != null && !result.IsNullOrEmpty() && !shoppingCart.IsTransient())
                {
                    //Evaluate promotions cart and apply rewards for available shipping methods
                    var promoEvalContext = shoppingCart.ToPromotionEvaluationContext();
                    await promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                    // Load from
                    var taxCalculationEnabled = context.UserContext.TryGetValue("taxCalculationEnabled", out var taxCalculationEnabledBoxed) && (bool)taxCalculationEnabledBoxed;
                    var fixedTaxRate = context.UserContext.TryGetValue("fixedTaxRate", out var fixedTaxRateBoxed) ? (decimal)fixedTaxRateBoxed : 0M;

                    //Evaluate taxes for available Shippings
                    var taxEvalContext = shoppingCart.ToTaxEvalContext(taxCalculationEnabled, fixedTaxRate);
                    taxEvalContext.Lines.Clear();
                    taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                    await taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
                }

                return result;
            });

            // Payment
            Field<MoneyType>("paymentPrice", resolve: context => context.Source.PaymentPrice);
            Field<MoneyType>("paymentPriceWithTax", resolve: context => context.Source.PaymentPriceWithTax);
            Field<MoneyType>("paymentTotal", resolve: context => context.Source.PaymentTotal);
            Field<MoneyType>("paymentTotalWithTax", resolve: context => context.Source.PaymentTotalWithTax);
            Field<ListGraphType<PaymentType>>("payments", resolve: context => context.Source.Payments);
            FieldAsync<ListGraphType<PaymentMethodType>>("availablePaymentMethods", resolve: async context =>
            {
                var shoppingCart = context.Source;

                if (string.IsNullOrEmpty(shoppingCart.StoreId))
                {
                    return Enumerable.Empty<PaymentMethod>();
                }

                var criteria = new PaymentMethodsSearchCriteria
                {
                    IsActive = true,
                    Take = int.MaxValue,
                    StoreId = shoppingCart.StoreId,
                };

                var payments = await paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);

                var result = payments.Results.Select(x => x.ToCartPaymentMethod(shoppingCart)).OrderBy(x => x.Priority).ToList();

                if (shoppingCart != null && !result.IsNullOrEmpty() && !shoppingCart.IsTransient())
                {
                    //Evaluate promotions cart and apply rewards for available shipping methods
                    var promoEvalContext = shoppingCart.ToPromotionEvaluationContext();
                    await promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                    // Load from
                    var taxCalculationEnabled = context.UserContext.TryGetValue("taxCalculationEnabled", out var taxCalculationEnabledBoxed) && (bool)taxCalculationEnabledBoxed;
                    var fixedTaxRate = context.UserContext.TryGetValue("fixedTaxRate", out var fixedTaxRateBoxed) ? (decimal)fixedTaxRateBoxed : 0M;

                    //Evaluate taxes for available payments
                    var taxEvalContext = shoppingCart.ToTaxEvalContext(taxCalculationEnabled, fixedTaxRate);
                    taxEvalContext.Lines.Clear();
                    taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                    await taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
                }

                return result;
            });
            Field<ListGraphType<PaymentPlanType>>("paymentPlan", resolve: context => context.Source.PaymentPlan);

            // Extended money
            Field<MoneyType>("extendedPriceTotal", resolve: context => context.Source.ExtendedPriceTotal);
            Field<MoneyType>("extendedPriceTotalWithTax", resolve: context => context.Source.ExtendedPriceTotalWithTax);

            // Handling totals
            Field<MoneyType>("handlingTotal", resolve: context => context.Source.HandlingTotal);
            Field<MoneyType>("handlingTotalWithTax", resolve: context => context.Source.HandlingTotalWithTax);

            // Discounts
            Field<MoneyType>("discountTotal", resolve: context => context.Source.DiscountTotal);
            Field<MoneyType>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);

            // Addresses
            Field<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);

            // Items
            Field<ListGraphType<LineItemType>>("items", resolve: context => context.Source.Items);
            Field(x => x.ItemsCount, nullable: true).Description("Count of different items");
            Field(x => x.ItemsQuantity, nullable: true).Description("Quantity of items");
            Field<LineItemType>("recentlyAddedItem", resolve: context => context.Source.RecentlyAddedItem);

            // Coupon
            Field<CopuponType>("coupon", resolve: context => context.Source.Coupon);
            Field<ListGraphType<CopuponType>>("coupons", resolve: context => context.Source.Coupons);

            // Other
            Field(x => x.ObjectType, nullable: true).Description("Object type");
            //Field<ListGraphType<DynamicPropertyType>>("dynamicProperties", resolve: context => context.Source.DynamicProperties); //todo add dynamic properties
            Field(x => x.IsValid, nullable: true).Description("Is cart valid");
            Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            Field(x => x.Type, nullable: true).Description("Shopping cart type");
        }
    }
}
