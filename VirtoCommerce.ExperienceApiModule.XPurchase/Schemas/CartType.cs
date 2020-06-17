using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Schemas
{
    public class CartType : ObjectGraphType<ShoppingCart>
    {
        public CartType()
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
            Field<ObjectGraphType<MoneyType>>("total", resolve: context => context.Source.Total);
            Field<ObjectGraphType<MoneyType>>("subTotal", resolve: context => context.Source.SubTotal);
            Field<ObjectGraphType<MoneyType>>("subTotalWithTax", resolve: context => context.Source.SubTotalWithTax);
            Field<ObjectGraphType<CurrencyType>>("currency", resolve: context => context.Source.Currency);
            Field<ObjectGraphType<MoneyType>>("taxTotal", resolve: context => context.Source.TaxTotal);
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Shipping tax type");
            Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);

            // Shipping
            Field<ObjectGraphType<MoneyType>>("shippingPrice", resolve: context => context.Source.ShippingPrice);
            Field<ObjectGraphType<MoneyType>>("shippingPriceWithTax", resolve: context => context.Source.ShippingPriceWithTax);
            Field<ObjectGraphType<MoneyType>>("shippingTotal", resolve: context => context.Source.ShippingTotal);
            Field<ObjectGraphType<MoneyType>>("shippingTotalWithTax", resolve: context => context.Source.ShippingTotalWithTax);
            Field<ListGraphType<ShipmentType>>("shipments", resolve: context => context.Source.Shipments);

            // Payment
            Field<ObjectGraphType<MoneyType>>("paymentPrice", resolve: context => context.Source.PaymentPrice);
            Field<ObjectGraphType<MoneyType>>("paymentPriceWithTax", resolve: context => context.Source.PaymentPriceWithTax);
            Field<ObjectGraphType<MoneyType>>("paymentTotal", resolve: context => context.Source.PaymentTotal);
            Field<ObjectGraphType<MoneyType>>("paymentTotalWithTax", resolve: context => context.Source.PaymentTotalWithTax);
            Field<ListGraphType<PaymentType>>("payments", resolve: context => context.Source.Payments);
            Field<ListGraphType<PaymentMethodType>>("availablePaymentMethods", resolve: context => context.Source.AvailablePaymentMethods);
            Field<ListGraphType<PaymentPlanType>>("paymentPlan", resolve: context => context.Source.PaymentPlan);

            // Extended money
            Field<ObjectGraphType<MoneyType>>("extendedPriceTotal", resolve: context => context.Source.ExtendedPriceTotal);
            Field<ObjectGraphType<MoneyType>>("extendedPriceTotalWithTax", resolve: context => context.Source.ExtendedPriceTotalWithTax);


            // Handling totals
            Field<ObjectGraphType<MoneyType>>("handlingTotal", resolve: context => context.Source.HandlingTotal);
            Field<ObjectGraphType<MoneyType>>("handlingTotalWithTax", resolve: context => context.Source.HandlingTotalWithTax);

            // Discounts
            Field<ObjectGraphType<MoneyType>>("discountTotal", resolve: context => context.Source.DiscountTotal);
            Field<ObjectGraphType<MoneyType>>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax);
            Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);

            // Addresses
            Field<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);

            // Items
            Field<ListGraphType<LineItemType>>("items", resolve: context => context.Source.Items);
            Field(x => x.ItemsCount, nullable: true).Description("Count of different items");
            Field(x => x.ItemsQuantity, nullable: true).Description("Quantity of items");
            Field<ObjectGraphType<LineItemType>>("recentlyAddedItem", resolve: context => context.Source.RecentlyAddedItem);

            // Coupon
            Field<ObjectGraphType<CopuponType>>("coupon", resolve: context => context.Source.Coupon);
            Field<ListGraphType<CopuponType>>("coupons", resolve: context => context.Source.Coupons);

            // Other
            Field(x => x.ObjectType, nullable: true).Description("Object type");
            Field(x => x.DynamicProperties, nullable: true).Description("Dynamic properties collections");
            Field<ListGraphType<DynamicPropertyType>>("dynamicProperties", resolve: context => context.Source.DynamicProperties);
            Field(x => x.IsValid, nullable: true).Description("Is cart valid");
            Field(x => x.ValidationErrors, nullable: true).Description("Validation errors");
            Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            Field(x => x.Type, nullable: true).Description("Shopping cart type");
        }
    }
}
