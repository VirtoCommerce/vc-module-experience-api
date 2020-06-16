using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Models;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.ValidationErrors;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Exceptions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing.Services;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Quote;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Security;
using VirtoCommerce.Platform.Core.Common;

using IEntity = VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common.IEntity;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Aggregates
{
    public class ShoppingCartAggregate : IShoppingCartAggregate
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICatalogService _catalogService;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly ITaxEvaluator _taxEvaluator;
        private readonly ICartService _cartService;

        public ShoppingCartAggregate(IShoppingCartService shoppingCartService,
            ICatalogService catalogSearchService,
            IPromotionEvaluator promotionEvaluator,
            ITaxEvaluator taxEvaluator,
            ICartService cartService,
            ShoppingCartContext context)
        {
            _shoppingCartService = shoppingCartService;
            _catalogService = catalogSearchService;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
            _cartService = cartService;
            Context = context;
        }

        #region ICartBuilder Members

        public virtual ShoppingCart Cart { get; protected set; }

        protected virtual ShoppingCartContext Context { get; set; }

        public virtual async Task TakeCartAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            //Load products for cart line items
            if (cart.Items.Any())
            {
                var productIds = cart.Items.Select(i => i.ProductId).ToArray();
                var products = await _catalogService.GetProductsAsync(productIds, Context.Currency, Context.Language, ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory | ItemResponseGroup.Outlines);
                foreach (var item in cart.Items)
                {
                    item.Product = products.FirstOrDefault(x => x.Id.EqualsInvariant(item.ProductId));
                }
            }

            Cart = cart;
        }

        public virtual Task UpdateCartComment(string comment)
        {
            EnsureCartExists();

            Cart.Comment = comment;

            return Task.CompletedTask;
        }

        public virtual async Task<bool> AddItemAsync(Product product, int quantity)
        {
            EnsureCartExists();

            var isProductAvailable = new ProductIsAvailableSpecification(product).IsSatisfiedBy(quantity);
            if (isProductAvailable)
            {
                var lineItem = product.ToLineItem(Cart.Language, quantity);
                lineItem.Product = product;
                await AddLineItemAsync(lineItem);
            }
            return isProductAvailable;
        }

        public virtual async Task ChangeItemQuantityByIdAsync(string lineItemId, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }
        }

        public virtual async Task ChangeItemQuantityByIndexAsync(int lineItemIndex, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.ElementAt(lineItemIndex);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }
        }

        public virtual async Task ChangeItemsQuantitiesAsync(int[] quantities)
        {
            EnsureCartExists();

            for (var i = 0; i < quantities.Length; i++)
            {
                var lineItem = Cart.Items.ElementAt(i);
                if (lineItem != null && quantities[i] > 0)
                {
                    await ChangeItemQuantityAsync(lineItem, quantities[i]);
                }
            }
        }

        public virtual Task RemoveItemAsync(string lineItemId)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                Cart.Items.Remove(lineItem);
            }

            return Task.FromResult((object)null);
        }

        public virtual Task AddCouponAsync(string couponCode)
        {
            EnsureCartExists();
            if (!Cart.Coupons.Any(c => c.Code.EqualsInvariant(couponCode)))
            {
                Cart.Coupons.Add(new Coupon { Code = couponCode });
            }

            return Task.FromResult((object)null);
        }

        public virtual Task RemoveCouponAsync(string couponCode = null)
        {
            EnsureCartExists();
            if (string.IsNullOrEmpty(couponCode))
            {
                Cart.Coupons.Clear();
            }
            else
            {
                Cart.Coupons.Remove(Cart.Coupons.FirstOrDefault(c => c.Code.EqualsInvariant(couponCode)));
            }

            return Task.FromResult((object)null);
        }

        public virtual Task ClearAsync()
        {
            EnsureCartExists();
            Cart.Items.Clear();
            return Task.FromResult((object)null);
        }

        public virtual async Task AddOrUpdateShipmentAsync(Shipment shipment)
        {
            EnsureCartExists();

            await RemoveExistingShipmentAsync(shipment);

            shipment.Currency = Cart.Currency;
            if (shipment.DeliveryAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address
                shipment.DeliveryAddress.Key = null;
            }
            Cart.Shipments.Add(shipment);

            if (string.IsNullOrEmpty(shipment.ShipmentMethodCode) || Cart.IsTransient())
            {
                return;
            }

            var availableShippingMethods = await GetAvailableShippingMethodsAsync();
            var shippingMethod = availableShippingMethods
                .FirstOrDefault(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShipmentMethodCode)
                    && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));

            if (shippingMethod == null)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unknown shipment method: {0} with option: {1}", shipment.ShipmentMethodCode, shipment.ShipmentMethodOption));
            }

            shipment.Price = shippingMethod.Price;
            shipment.DiscountAmount = shippingMethod.DiscountAmount;
            shipment.TaxType = shippingMethod.TaxType;
        }

        public virtual Task RemoveShipmentAsync(string shipmentId)
        {
            EnsureCartExists();

            var shipment = Cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                Cart.Shipments.Remove(shipment);
            }

            return Task.FromResult((object)null);
        }

        public virtual async Task AddOrUpdatePaymentAsync(Payment payment)
        {
            EnsureCartExists();

            await RemoveExistingPaymentAsync(payment);
            if (payment.BillingAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address
                payment.BillingAddress.Key = null;
            }
            Cart.Payments.Add(payment);

            if (!string.IsNullOrEmpty(payment.PaymentGatewayCode) && !Cart.IsTransient())
            {
                var availablePaymentMethods = await GetAvailablePaymentMethodsAsync();
                var paymentMethod = availablePaymentMethods.FirstOrDefault(pm => string.Equals(pm.Code, payment.PaymentGatewayCode, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethod == null)
                {
                    throw new Exception("Unknown payment method " + payment.PaymentGatewayCode);
                }
            }
        }

        public virtual async Task MergeWithCartAsync(ShoppingCart cart)
        {
            EnsureCartExists();

            //Reset primary keys for all aggregated entities before merge
            //To prevent insertions same Ids for target cart
            //exclude user because it might be the current one
            var entities = cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities.Where(x => !(x is User)).ToList())
            {
                entity.Id = null;
            }

            foreach (var lineItem in cart.Items)
            {
                await AddLineItemAsync(lineItem);
            }

            foreach (var coupon in cart.Coupons)
            {
                await AddCouponAsync(coupon.Code);
            }

            foreach (var shipment in cart.Shipments)
            {
                await AddOrUpdateShipmentAsync(shipment);
            }

            foreach (var payment in cart.Payments)
            {
                await AddOrUpdatePaymentAsync(payment);
            }
        }

        public virtual async Task RemoveCartAsync()
        {
            EnsureCartExists();
            await _shoppingCartService.DeleteAsync(new[] { Cart.Id });
        }

        public virtual async Task FillFromQuoteRequestAsync(QuoteRequest quoteRequest)
        {
            EnsureCartExists();

            var productIds = quoteRequest.Items.Select(i => i.ProductId).ToArray();
            var products = await _catalogService.GetProductsAsync(productIds, Context.Currency, Context.Language, ItemResponseGroup.ItemLarge);

            Cart.Items.Clear();
            foreach (var product in products)
            {
                var quoteItem = quoteRequest.Items.FirstOrDefault(i => i.ProductId == product.Id);
                if (quoteItem != null)
                {
                    var lineItem = product.ToLineItem(Cart.Language, (int)quoteItem.SelectedTierPrice.Quantity);
                    lineItem.Product = product;
                    lineItem.ListPrice = quoteItem.ListPrice;
                    lineItem.SalePrice = quoteItem.SelectedTierPrice.Price;
                    if (lineItem.ListPrice < lineItem.SalePrice)
                    {
                        lineItem.ListPrice = lineItem.SalePrice;
                    }
                    lineItem.DiscountAmount = lineItem.ListPrice - lineItem.SalePrice;
                    lineItem.IsReadOnly = true;
                    lineItem.Id = null;
                    Cart.Items.Add(lineItem);
                }
            }

            if (quoteRequest.RequestShippingQuote)
            {
                Cart.Shipments.Clear();
                var shipment = new Shipment(Cart.Currency);

                if (quoteRequest.ShippingAddress != null)
                {
                    shipment.DeliveryAddress = quoteRequest.ShippingAddress;
                }

                if (quoteRequest.ShipmentMethod != null)
                {
                    var availableShippingMethods = await GetAvailableShippingMethodsAsync();
                    var availableShippingMethod = availableShippingMethods?.FirstOrDefault(sm => sm.ShipmentMethodCode == quoteRequest.ShipmentMethod.ShipmentMethodCode);

                    if (availableShippingMethod != null)
                    {
                        shipment = quoteRequest.ShipmentMethod.ToCartShipment(Cart.Currency);
                    }
                }
                Cart.Shipments.Add(shipment);
            }

            var payment = new Payment(Cart.Currency)
            {
                Amount = quoteRequest.Totals.GrandTotalInclTax
            };

            if (quoteRequest.BillingAddress != null)
            {
                payment.BillingAddress = quoteRequest.BillingAddress;
            }

            Cart.Payments.Clear();
            Cart.Payments.Add(payment);
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            //Request available shipping rates
            
            var result = await _cartService.GetAvailableShippingMethodsAsync(Cart, Context.CurrentStore.Id);
            if (!result.IsNullOrEmpty())
            {
                //Evaluate promotions cart and apply rewards for available shipping methods
                var promoEvalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                //Evaluate taxes for available shipping rates
                var taxEvalContext = Cart.ToTaxEvalContext(Context.CurrentStore.TaxCalculationEnabled, Context.CurrentStore.FixedTaxRate);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            }
            return result;
        }

        public virtual async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            EnsureCartExists();

            var result = await _cartService.GetAvailablePaymentMethodsAsync(Cart, Context.CurrentStore.Id);

            if (!result.IsNullOrEmpty())
            {
                //Evaluate promotions cart and apply rewards for available shipping methods
                var promoEvalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                //Evaluate taxes for available payments
                var taxEvalContext = Cart.ToTaxEvalContext(Context.CurrentStore.TaxCalculationEnabled, Context.CurrentStore.FixedTaxRate);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            }
            return result;
        }

        public async Task ValidateAsync()
        {
            EnsureCartExists();
            await Task.WhenAll(ValidateCartItemsAsync(), ValidateCartShipmentsAsync());
            Cart.IsValid = Cart.Items.All(x => x.IsValid) && Cart.Shipments.All(x => x.IsValid);
        }

        public virtual async Task EvaluatePromotionsAsync()
        {
            EnsureCartExists();

            var isReadOnlyLineItems = Cart.Items.Any(i => i.IsReadOnly);
            if (!isReadOnlyLineItems)
            {
                //Get product inventory to fill InStockQuantity parameter of LineItem
                //required for some promotions evaluation

                foreach (var lineItem in Cart.Items.Where(x => x.Product != null).ToList())
                {
                    lineItem.InStockQuantity = (int)lineItem.Product.AvailableQuantity;
                }

                var evalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(evalContext, new IDiscountable[] { Cart });
            }
        }

        public async Task EvaluateTaxesAsync()
        {
            await _taxEvaluator.EvaluateTaxesAsync(Cart.ToTaxEvalContext(Context.CurrentStore.TaxCalculationEnabled, Context.CurrentStore.FixedTaxRate), new[] { Cart });
        }

        public virtual async Task SaveAsync()
        {
            EnsureCartExists();

            await EvaluatePromotionsAsync();
            await EvaluateTaxesAsync();

            // todo: implement!
            //var cart = await _cartService.SaveChanges(Cart);
            //await TakeCartAsync(cart);
        }

        #endregion ICartBuilder Members

        protected virtual Task ValidateCartItemsAsync()
        {
            foreach (var lineItem in Cart.Items.ToList())
            {
                lineItem.ValidationErrors.Clear();

                if (lineItem.Product == null || !lineItem.Product.IsActive || !lineItem.Product.IsBuyable)
                {
                    lineItem.ValidationErrors.Add(new UnavailableError());
                }
                else
                {
                    var isProductAvailable = new ProductIsAvailableSpecification(lineItem.Product).IsSatisfiedBy(lineItem.Quantity);
                    if (!isProductAvailable)
                    {
                        var availableQuantity = lineItem.Product.AvailableQuantity;
                        lineItem.ValidationErrors.Add(new QuantityError(availableQuantity));
                    }

                    var tierPrice = lineItem.Product.Price.GetTierPrice(lineItem.Quantity);
                    if (tierPrice.Price > lineItem.SalePrice)
                    {
                        lineItem.ValidationErrors.Add(new PriceError(lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price, tierPrice.PriceWithTax));
                    }
                }

                lineItem.IsValid = !lineItem.ValidationErrors.Any();
            }
            return Task.CompletedTask;
        }

        protected virtual async Task ValidateCartShipmentsAsync()
        {
            foreach (var shipment in Cart.Shipments.ToArray())
            {
                shipment.ValidationErrors.Clear();

                var availShippingmethods = await GetAvailableShippingMethodsAsync();
                var shipmentShippingMethod = availShippingmethods.FirstOrDefault(sm => shipment.HasSameMethod(sm));
                if (shipmentShippingMethod == null)
                {
                    shipment.ValidationErrors.Add(new UnavailableError());
                }
                else if (shipmentShippingMethod.Price != shipment.Price)
                {
                    shipment.ValidationErrors.Add(new PriceError(shipment.Price, shipment.PriceWithTax, shipmentShippingMethod.Price, shipmentShippingMethod.PriceWithTax));
                }
            }
        }

        protected virtual Task RemoveExistingPaymentAsync(Payment payment)
        {
            if (payment != null)
            {
                var existingPayment = !payment.IsTransient() ? Cart.Payments.FirstOrDefault(s => s.Id == payment.Id) : null;
                if (existingPayment != null)
                {
                    Cart.Payments.Remove(existingPayment);
                }
            }

            return Task.FromResult((object)null);
        }

        protected virtual Task RemoveExistingShipmentAsync(Shipment shipment)
        {
            if (shipment != null)
            {
                var existShipment = !shipment.IsTransient() ? Cart.Shipments.FirstOrDefault(s => s.Id == shipment.Id) : null;
                if (existShipment != null)
                {
                    Cart.Shipments.Remove(existShipment);
                }
            }

            return Task.FromResult((object)null);
        }

        protected virtual Task ChangeItemQuantityAsync(LineItem lineItem, int quantity)
        {
            if (lineItem != null && !lineItem.IsReadOnly)
            {
                if (lineItem.Product != null)
                {
                    var salePrice = lineItem.Product.Price.GetTierPrice(quantity).Price;
                    if (salePrice != 0)
                    {
                        lineItem.SalePrice = salePrice;
                    }
                    //List price should be always greater ot equals sale price because it may cause incorrect totals calculation
                    if (lineItem.ListPrice < lineItem.SalePrice)
                    {
                        lineItem.ListPrice = lineItem.SalePrice;
                    }
                }
                if (quantity > 0)
                {
                    lineItem.Quantity = quantity;
                }
                else
                {
                    Cart.Items.Remove(lineItem);
                }
            }
            return Task.CompletedTask;
        }

        protected virtual async Task AddLineItemAsync(LineItem lineItem)
        {
            var existingLineItem = Cart.Items.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                await ChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity));
            }
            else
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }
        }

        protected virtual void EnsureCartExists()
        {
            if (Cart == null)
            {
                throw new CartException("Cart not loaded.");
            }
        }
    }
}
