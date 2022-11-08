using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public static class CartErrorDescriber
    {
        public static CartValidationError LineItemIsReadOnly(IEntity entity)
        {
            var result = new CartValidationError(entity, $"Line item is read only", "LINE_ITEM_IS_READ_ONLY");
            return result;
        }

        public static CartValidationError LineItemWithGivenIdNotFound(IEntity entity)
        {
            var result = new CartValidationError(entity, $"Line item with {entity.Id} not found", "LINE_ITEM_NOT_FOUND");
            return result;
        }

        public static CartValidationError UnableToSetLessPrice(IEntity entity)
        {
            var result = new CartValidationError(entity, "Unable to set less price", "UNABLE_SET_LESS_PRICE");
            return result;
        }

        public static CartValidationError ProductPriceChangedError(IEntity entity, decimal oldPrice, decimal oldPriceWithTax, decimal newPrice, decimal newPriceWithTax)
        {
            var result = new CartValidationError(entity, $"The product price is changed the new price is {newPrice}", "PRODUCT_PRICE_CHANGED")
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    ["old_price"] = oldPrice,
                    ["old_price_with_tax"] = oldPriceWithTax,
                    ["new_price"] = newPrice,
                    ["new_price_with_tax"] = newPriceWithTax
                }
            };

            return result;
        }

        public static CartValidationError ProductQtyChangedError(IEntity entity, long availQty)
        {
            var result = new CartValidationError(entity, "The product available qty is changed", "PRODUCT_QTY_CHANGED");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["availQty"] = availQty
            };
            return result;
        }

        public static CartValidationError ProductQtyInsufficientError(IEntity entity, long newQty, long availQty)
        {
            var result = new CartValidationError(entity, $"The product available quantity {availQty} is insufficient for requested {newQty}", "PRODUCT_QTY_INSUFFICIENT");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["new_qty"] = newQty,
                ["availQty"] = availQty
            };
            return result;
        }

        public static CartValidationError ShipmentMethodPriceChanged(IEntity entity, decimal oldPrice, decimal oldPriceWithTax, decimal newPrice, decimal newPriceWithTax)
        {
            var result = new CartValidationError(entity, "The shipment method price is changed", "SHIPMENT_METHOD_PRICE_CHANGED")
            {
                FormattedMessagePlaceholderValues = new Dictionary<string, object>
                {
                    ["old_price"] = oldPrice,
                    ["old_price_with_tax"] = oldPriceWithTax,
                    ["new_price"] = newPrice,
                    ["new_price_with_tax"] = newPriceWithTax
                }
            };
            return result;
        }

        public static CartValidationError PaymentMethodUnavailable(IEntity entity, string name)
        {
            var result = new CartValidationError(entity, $"The payment method {name} unavailable", "PAYMENT_METHOD_UNAVAILABLE");
            return result;
        }

        public static CartValidationError ShipmentMethodUnavailable(IEntity entity, string shipmentMethodCode, string shipmentMethodOption)
        {
            var result = new CartValidationError(entity, $"The shipment method {shipmentMethodCode ?? "NULLShipmentMethodCode".ToUpper()} with {shipmentMethodOption ?? "NULLshipmentMethodOption".ToUpper()} unavailable", "SHIPMENT_METHOD_UNAVAILABLE");
            return result;
        }

        public static CartValidationError ProductInvalidError(string type, string sku)
        {
            var result = new CartValidationError(type, sku, $"Product with SKU {sku} was not added to cart. This SKU doesn't exist.", "CART_INVALID_PRODUCT");
            return result;
        }

        public static CartValidationError ProductUnavailableError(IEntity entity)
        {
            var result = new CartValidationError(entity, "The product is not longer available for purchase", "CART_PRODUCT_UNAVAILABLE");
            return result;
        }

        public static CartValidationError ProductUnavailableError(string type, string sku)
        {
            var result = new CartValidationError(type, sku, $"Product with SKU {sku} was not added to cart. The product is not longer available for purchase.", "CART_PRODUCT_UNAVAILABLE");
            return result;
        }

        public static CartValidationError ProductInactiveError(string type, string sku)
        {
            var result = new CartValidationError(type, sku, $"Product with SKU {sku} was not added to cart. The product is inactive.", "CART_PRODUCT_INACTIVE");
            return result;
        }

        public static CartValidationError ProductNoPriceError(string type, string sku)
        {
            var result = new CartValidationError(type, sku, $"Product with SKU {sku} was not added to cart. Price is invalid.", "PRODUCT_PRICE_INVALID");
            return result;
        }

        public static CartValidationError ProductAvailableQuantityError(string type, string sku, int qty, long availableQty)
        {
            var result = new CartValidationError(type, sku, $"Product with SKU {sku} was not added to cart. Maximum available qunantity is {availableQty}.", "PRODUCT_FFC_QTY");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["qty"] = qty,
                ["availableQty"] = availableQty
            };

            return result;
        }

        public static CartValidationError ProductMinQuantityError(IEntity entity, int qty, int minQty)
        {
            var result = new CartValidationError(entity, $"Product quantity {qty} is less than minumum {minQty}", "PRODUCT_MIN_QTY");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["qty"] = qty,
                ["minQty"] = minQty
            };

            return result;
        }

        public static CartValidationError ProductMaxQuantityError(IEntity entity, int qty, int maxQty)
        {
            var result = new CartValidationError(entity, $"Product quantity {qty} is greater than maximum {maxQty}", "PRODUCT_MAX_QTY");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["qty"] = qty,
                ["maxQty"] = maxQty
            };

            return result;
        }

        public static CartValidationError ProductMinQuantityError(string type, string code, int qty, int minQty)
        {
            var result = new CartValidationError(type, code, $"Product {code} quantity {qty} is less than minumum {minQty}", "PRODUCT_MIN_QTY");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["qty"] = qty,
                ["minQty"] = minQty
            };

            return result;
        }

        public static CartValidationError ProductMaxQuantityError(string type, string code, int qty, int maxQty)
        {
            var result = new CartValidationError(type, code, $"Product {code} quantity {qty} is greater than maximum {maxQty}", "PRODUCT_MAX_QTY");
            result.FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                ["qty"] = qty,
                ["maxQty"] = maxQty
            };

            return result;
        }
    }
}
