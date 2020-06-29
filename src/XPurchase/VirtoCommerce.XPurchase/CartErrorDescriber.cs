using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public static class CartErrorDescriber
    {
        public static CartValidationError UnableToSetLessPrice(IEntity entity)
        {
            var result = new CartValidationError(entity, "Unable to set less price", "UNABLE_SET_LESS_PRICE");
            return result;
        }

        public static CartValidationError ProductPriceChangedError(IEntity entity, decimal oldPrice, decimal oldPriceWithTax, decimal newPrice, decimal newPriceWithTax)
        {
            var result = new CartValidationError(entity, "The product price is changed", "PRODUCT_PRICE_CHANGED");
            result.FormattedMessagePlaceholderValues["old_price"] = oldPrice;
            result.FormattedMessagePlaceholderValues["old_price_with_tax"] = oldPriceWithTax;
            result.FormattedMessagePlaceholderValues["new_price"] = newPrice;
            result.FormattedMessagePlaceholderValues["new_price_with_tax"] = newPriceWithTax;
            return result;
        }

        public static CartValidationError ProductQtyChangedError(IEntity entity, long newQty)
        {
            var result = new CartValidationError(entity, "The product available qty is changed", "PRODUCT_QTY_CHANGED");
            result.FormattedMessagePlaceholderValues["new_qty"] = newQty;
            return result;
        }

        public static CartValidationError ProductUnavailableError(IEntity entity)
        {
            var result = new CartValidationError(entity, "The product is not longer available for purchase", "CART_PRODUCT_UNAVAILABLE");
            return result;
        }

        public static CartValidationError ShipmentMethodPriceChanged(IEntity entity, decimal oldPrice, decimal oldPriceWithTax, decimal newPrice, decimal newPriceWithTax)
        {
            var result = new CartValidationError(entity, "The shipment method price is changed", "SHIPMENT_METHOD_PRICE_CHANGED");
            result.FormattedMessagePlaceholderValues["old_price"] = oldPrice;
            result.FormattedMessagePlaceholderValues["old_price_with_tax"] = oldPriceWithTax;
            result.FormattedMessagePlaceholderValues["new_price"] = newPrice;
            result.FormattedMessagePlaceholderValues["new_price_with_tax"] = newPriceWithTax;
            return result;
        }

        public static CartValidationError ShipmentMethodUnavailable(IEntity entity)
        {
            var result = new CartValidationError(entity, "The shipment method is no longer available", "SHIPMENT_METHOD_UNAVAILABLE");
            return result;
        }


    }
}
