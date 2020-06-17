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
            Field(x => x.Total, nullable: true).Description("Shopping cart total cost");
            Field(x => x.SubTotal, nullable: true).Description("Shopping cart subtotal");
            Field(x => x.SubTotalWithTax, nullable: true).Description("Shopping with taxes");
            Field(x => x.Currency, nullable: true).Description("Currency");
            Field(x => x.TaxTotal, nullable: true).Description("TaxTotal");
            Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            Field(x => x.TaxType, nullable: true).Description("Shipping tax type");
            Field(x => x.TaxDetails, nullable: true).Description("Collection of TaxDetail objects"); // Reslove?

            // Shipping
            Field(x => x.ShippingPrice, nullable: true).Description("Sum shipping cost without discount");
            Field(x => x.ShippingPriceWithTax, nullable: true).Description("Shipping cost with tax");
            Field(x => x.ShippingTotal, nullable: true).Description("Shipping total cost");
            Field(x => x.ShippingTotalWithTax, nullable: true).Description("Shipping total cost with tax");
            Field(x => x.Shipments, nullable: true).Description("Collection of shipments objects");

            // Payment
            Field(x => x.PaymentPrice, nullable: true).Description("Sum cost of payments");
            Field(x => x.PaymentPriceWithTax, nullable: true).Description("Sum cost of payments with tax");
            Field(x => x.PaymentTotal, nullable: true).Description("Payments total cost");
            Field(x => x.PaymentTotalWithTax, nullable: true).Description("Payments total cost with tax");
            Field(x => x.Payments, nullable: true).Description("Collection of payment objects");
            Field(x => x.AvailablePaymentMethods, nullable: true).Description("Available payment methods");
            Field(x => x.PaymentPlan, nullable: true).Description("Future subscription payment plan");

            // Extended money
            Field(x => x.ExtendedPriceTotal, nullable: true).Description("Shopping cart items total extended price");
            Field(x => x.ExtendedPriceTotalWithTax, nullable: true).Description("Shopping cart items total extended price with tax");

            // Handling totals
            Field(x => x.HandlingTotal, nullable: true).Description("Value of handling total cost");
            Field(x => x.HandlingTotalWithTax, nullable: true).Description("Value of handling total cost with tax");

            // Discounts
            Field(x => x.DiscountTotal, nullable: true).Description("Total discount amount");
            Field(x => x.DiscountTotalWithTax, nullable: true).Description("Total discount amount with tax");
            Field(x => x.Discounts, nullable: true).Description("Collection of discounts");

            // Addresses
            Field(x => x.Addresses, nullable: true).Description("Collection of address");

            // Items
            Field(x => x.Items, nullable: true).Description("Collection of items");
            Field(x => x.ItemsCount, nullable: true).Description("Count of different items");
            Field(x => x.ItemsQuantity, nullable: true).Description("Quantity of items");
            Field(x => x.RecentlyAddedItem, nullable: true).Description("Recently added item");

            // Coupon
            Field(x => x.Coupon, nullable: true).Description("First coupon");
            Field(x => x.Coupons, nullable: true).Description("Collection  of shopping cart coupons");

            // Other
            Field(x => x.ObjectType, nullable: true).Description("Object type");
            Field(x => x.DynamicProperties, nullable: true).Description("Dynamic properties collections");
            Field(x => x.IsValid, nullable: true).Description("Is cart valid");
            Field(x => x.ValidationErrors, nullable: true).Description("Validation errors");
            Field(x => x.Type, nullable: true).Description("Shopping cart type");
        }
    }
}
