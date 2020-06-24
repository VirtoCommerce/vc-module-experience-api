using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Models.OperationResults;
using VirtoCommerce.XPurchase.Models.Quote;

namespace VirtoCommerce.XPurchase.Models.Cart.Services
{
    /// <summary>
    /// Represent abstraction for working with customer shopping cart
    /// </summary>
    public interface IShoppingCartAggregate
    {
        ShoppingCart Cart { get; }

        /// <summary>
        ///  Capture cart and all next changes will be implemented on it
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task<OperationResult> TakeCartAsync(ShoppingCart cart);

        /// <summary>
        /// Update shopping cart comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<OperationResult> UpdateCartComment(string comment);

        /// <summary>
        /// Add new product to cart
        /// </summary>
        Task<OperationResult> AddItemAsync(AddCartItem command);

        /// <summary>
        /// Change cart item qty by product index
        /// </summary>
        /// <param name="lineItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<OperationResult> ChangeItemQuantityByIdAsync(string lineItemId, int quantity);

        /// <summary>
        /// Change cart item qty by item id
        /// </summary>
        /// <param name="lineItemIndex"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<OperationResult> ChangeItemQuantityByIndexAsync(int lineItemIndex, int quantity);

        Task<OperationResult> ChangeItemsQuantitiesAsync(int[] quantities);

        /// <summary>
        /// Remove item from cart by id
        /// </summary>
        /// <param name="lineItemId"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveItemAsync(string lineItemId);

        /// <summary>
        /// Apply marketing coupon to captured cart
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        Task<OperationResult> AddCouponAsync(string couponCode);

        /// <summary>
        /// remove exist coupon from cart
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveCouponAsync(string couponCode = null);

        /// <summary>
        /// Clear cart remove all items and shipments and payments
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> ClearAsync();

        /// <summary>
        /// Add or update shipment to cart
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        Task<OperationResult> AddOrUpdateShipmentAsync(Shipment shipment);

        /// <summary>
        /// Remove exist shipment from cart
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        Task<OperationResult> RemoveShipmentAsync(string shipmentId);

        /// <summary>
        /// Add or update payment in cart
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task<OperationResult> AddOrUpdatePaymentAsync(Payment payment);

        /// <summary>
        /// Merge other cart with captured
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task<OperationResult> MergeWithCartAsync(ShoppingCart cart);

        /// <summary>
        /// Remove cart from service
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> RemoveCartAsync();

        /// <summary>
        /// Fill current captured cart from RFQ
        /// </summary>
        /// <param name="quoteRequest"></param>
        /// <returns></returns>
        Task<OperationResult> FillFromQuoteRequestAsync(QuoteRequest quoteRequest);

        /// <summary>
        /// Returns all available shipment methods for current cart
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync();

        /// <summary>
        /// Returns all available payment methods for current cart
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync();

        /// <summary>
        /// Evaluate marketing discounts for captured cart
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> EvaluatePromotionsAsync();

        /// <summary>
        /// Evaluate taxes  for captured cart
        /// </summary>
        /// <returns></returns>
        Task<OperationResult> EvaluateTaxesAsync();

        Task<OperationResult> ValidateAsync();

        Task<OperationResult> SaveAsync();
    }
}
