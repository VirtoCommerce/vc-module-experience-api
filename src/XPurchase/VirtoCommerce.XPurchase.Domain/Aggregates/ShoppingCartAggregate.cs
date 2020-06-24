using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Converters;
using VirtoCommerce.XPurchase.Domain.Models;
using VirtoCommerce.XPurchase.Models;
using VirtoCommerce.XPurchase.Models.Cart;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Enums;
using VirtoCommerce.XPurchase.Models.Exceptions;
using VirtoCommerce.XPurchase.Models.Marketing;
using VirtoCommerce.XPurchase.Models.Marketing.Services;
using VirtoCommerce.XPurchase.Models.OperationResults;
using VirtoCommerce.XPurchase.Models.Quote;
using VirtoCommerce.XPurchase.Models.Security;
using VirtoCommerce.XPurchase.Models.Validators;
using IEntity = VirtoCommerce.XPurchase.Models.Common.IEntity;

namespace VirtoCommerce.XPurchase.Domain.Aggregates
{
    public class ShoppingCartAggregate : IShoppingCartAggregate
    {
        private readonly IProductsRepository _catalogService;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITaxEvaluator _taxEvaluator;

        public ShoppingCartAggregate(
            IProductsRepository catalogSearchService,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IPromotionEvaluator promotionEvaluator,
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShoppingCartService shoppingCartService,
            ITaxEvaluator taxEvaluator,
            ShoppingCartContext context)
        {
            _catalogService = catalogSearchService;
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _promotionEvaluator = promotionEvaluator;
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _shoppingCartService = shoppingCartService;
            _taxEvaluator = taxEvaluator;
            Context = context;
        }

        public virtual ShoppingCart Cart { get; protected set; }

        protected virtual ShoppingCartContext Context { get; set; }

        public virtual async Task<OperationResult> TakeCartAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                return new ErrorResult(ErrorType.Critical, "Shopping cart is null");
            }

            //Load products for cart line items
            if (cart.Items.Any())
            {
                var productIds = cart.Items.Select(i => i.ProductId).ToArray();
                var products = await _catalogService.GetProductsAsync(productIds, Context.Currency, Context.Language, ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory | ItemResponseGroup.Outlines);
                foreach (var item in cart.Items)
                {
                    item.Product = products?.FirstOrDefault(x => x.Id.EqualsInvariant(item.ProductId));
                }
            }

            Cart = cart;

            return new SuccessResult();
        }

        public virtual Task<OperationResult> UpdateCartComment(string comment)
        {
            EnsureCartExists();

            Cart.Comment = comment;

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        public virtual async Task<OperationResult> AddItemAsync(AddCartItem command)
        {
            EnsureCartExists();

            var result = await new AddCartItemValidator(Cart).ValidateAsync(command, ruleSet: Cart.ValidationRuleSet);
            if (!result.IsValid)
            {
                return new ErrorResult(ErrorType.Critical, string.Join(" ", result.Errors.Select(x => x.ErrorMessage)));
            }

            var lineItem = command.Product.ToLineItem(Cart.Language, command.Quantity);
            lineItem.Product = command.Product;
            if (command.Price != null)
            {
                var listPrice = new Money(command.Price.Value, Cart.Currency);
                lineItem.ListPrice = listPrice;
                lineItem.SalePrice = listPrice;
            }

            if (!string.IsNullOrEmpty(command.Comment))
            {
                lineItem.Comment = command.Comment;
            }

            if (!command.DynamicProperties.IsNullOrEmpty())
            {
                lineItem.DynamicProperties = new MutablePagedList<DynamicProperty>(command.DynamicProperties.Select(x => new DynamicProperty
                {
                    Name = x.Key,
                    Values = new[] { new LocalizedString { Language = Cart.Language, Value = x.Value } }
                }));
            }

            await AddLineItemAsync(lineItem);

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> ChangeItemQuantityByIdAsync(string lineItemId, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == lineItemId);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> ChangeItemQuantityByIndexAsync(int lineItemIndex, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.ElementAt(lineItemIndex);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> ChangeItemsQuantitiesAsync(int[] quantities)
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

            return new SuccessResult();
        }

        public virtual Task<OperationResult> RemoveItemAsync(string lineItemId)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                Cart.Items.Remove(lineItem);
            }

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        public virtual Task<OperationResult> AddCouponAsync(string couponCode)
        {
            EnsureCartExists();
            if (!Cart.Coupons.Any(c => c.Code.EqualsInvariant(couponCode)))
            {
                Cart.Coupons.Add(new Coupon { Code = couponCode });
            }

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        public virtual Task<OperationResult> RemoveCouponAsync(string couponCode = null)
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

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        public virtual async Task<OperationResult> ClearAsync()
        {
            EnsureCartExists();

            Cart.Items.Clear();

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> AddOrUpdateShipmentAsync(Shipment shipment)
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
                return new ErrorResult(ErrorType.Info, "Shipment method code is null or cart is transient");
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

            return new SuccessResult();
        }

        public virtual Task<OperationResult> RemoveShipmentAsync(string shipmentId)
        {
            EnsureCartExists();

            var shipment = Cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                Cart.Shipments.Remove(shipment);
            }

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        public virtual async Task<OperationResult> AddOrUpdatePaymentAsync(Payment payment)
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

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> MergeWithCartAsync(ShoppingCart cart)
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

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> RemoveCartAsync()
        {
            EnsureCartExists();

            await _shoppingCartService.DeleteAsync(new[] { Cart.Id });

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> FillFromQuoteRequestAsync(QuoteRequest quoteRequest)
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

            return new SuccessResult();
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            //Request available shipping rates

            var result = await GetAvailableShippingMethodsAsync(Cart);
            if (!result.IsNullOrEmpty())
            {
                //Evaluate promotions cart and apply rewards for available shipping methods
                var promoEvalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                //Evaluate taxes for available shipping rates
                var taxEvalContext = Cart.ToTaxEvalContext(Context.Store.TaxCalculationEnabled, Context.Store.FixedTaxRate);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            }
            return result;
        }

        public virtual async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            EnsureCartExists();

            var result = await GetAvailablePaymentMethodsAsync(Cart);

            if (!result.IsNullOrEmpty())
            {
                //Evaluate promotions cart and apply rewards for available shipping methods
                var promoEvalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);

                //Evaluate taxes for available payments
                var taxEvalContext = Cart.ToTaxEvalContext(Context.Store.TaxCalculationEnabled, Context.Store.FixedTaxRate);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
                await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            }
            return result;
        }

        public async Task<OperationResult> ValidateAsync()
        {
            EnsureCartExists();

            var shippingMethods = await GetAvailableShippingMethodsAsync();

            var result = await new CartValidator(shippingMethods).ValidateAsync(Cart, ruleSet: Cart.ValidationRuleSet);

            Cart.IsValid = result.IsValid;

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> EvaluatePromotionsAsync()
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

            return new SuccessResult();
        }

        public async Task<OperationResult> EvaluateTaxesAsync()
        {
            await _taxEvaluator.EvaluateTaxesAsync(Cart.ToTaxEvalContext(Context.Store.TaxCalculationEnabled, Context.Store.FixedTaxRate), new[] { Cart });

            return new SuccessResult();
        }

        public virtual async Task<OperationResult> SaveAsync()
        {
            EnsureCartExists();

            var cartDto = Cart.ToShoppingCartDto();

            await _shoppingCartService.SaveChangesAsync(new CartModule.Core.Model.ShoppingCart[] { cartDto });

            return new SuccessResult();
        }

        protected virtual Task<OperationResult> RemoveExistingPaymentAsync(Payment payment)
        {
            if (payment != null)
            {
                var existingPayment = !payment.IsTransient() ? Cart.Payments.FirstOrDefault(s => s.Id == payment.Id) : null;
                if (existingPayment != null)
                {
                    Cart.Payments.Remove(existingPayment);
                }
            }

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        protected virtual Task<OperationResult> RemoveExistingShipmentAsync(Shipment shipment)
        {
            if (shipment != null)
            {
                var existShipment = !shipment.IsTransient() ? Cart.Shipments.FirstOrDefault(s => s.Id == shipment.Id) : null;
                if (existShipment != null)
                {
                    Cart.Shipments.Remove(existShipment);
                }
            }

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        protected virtual Task<OperationResult> ChangeItemQuantityAsync(LineItem lineItem, int quantity)
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

            return Task.FromResult<OperationResult>(new SuccessResult());
        }

        protected virtual async Task<OperationResult> AddLineItemAsync(LineItem lineItem)
        {
            var existingLineItem = Cart.Items.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                await ChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity));
                await ChangeItemPriceAsync(existingLineItem, new ChangeCartItemPrice() { LineItemId = existingLineItem.Id, NewPrice = lineItem.ListPrice.Amount });
                existingLineItem.Comment = lineItem.Comment;
                existingLineItem.DynamicProperties = lineItem.DynamicProperties;
            }
            else
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }

            return new SuccessResult();
        }

        public virtual async Task ChangeItemPriceAsync(LineItem lineItem, ChangeCartItemPrice changePrice)
        {
            await new ChangeCartItemPriceValidator(Cart).ValidateAndThrowAsync(changePrice, ruleSet: Cart.ValidationRuleSet);
            var newPriceMoney = new Money(changePrice.NewPrice, Cart.Currency);
            lineItem.ListPrice = newPriceMoney;
            lineItem.SalePrice = newPriceMoney;
        }

        protected virtual void EnsureCartExists()
        {
            if (Cart == null)
            {
                throw new CartException("Cart not loaded.");
            }
        }

        public async Task<OperationResult> DeleteCartByIdAsync(string cartId)
        {
            await _shoppingCartService.DeleteAsync(new[] { cartId ?? throw new ArgumentNullException(nameof(cartId)) });

            return new SuccessResult();
        }

        public async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrEmpty(cart.StoreId) || cart.IsTransient())
            {
                return Enumerable.Empty<PaymentMethod>();
            }

            var criteria = new PaymentMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = cart.StoreId,
            };

            var payments = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);

            return payments.Results.Select(x => x.ToCartPaymentMethod(cart)).OrderBy(x => x.Priority).ToList();
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart)
        {
            if (cart == null || string.IsNullOrEmpty(cart.StoreId) || cart.IsTransient())
            {
                return Enumerable.Empty<ShippingMethod>();
            }

            var criteria = new ShippingModule.Core.Model.Search.ShippingMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = cart.StoreId,
            };

            var shippingRates = await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);

            return shippingRates.Results
                .Select(x => x.ToShippingMethod(cart.Currency))
                .OrderBy(x => x.Priority)
                .ToList();
        }

        public async Task<ShoppingCart> GetByIdAsync(string cartId, Currency currency, string userId)
        {
            var cartDto = await _shoppingCartService.GetByIdAsync(cartId, CartModule.Core.Model.CartResponseGroup.Full.ToString());
            if (cartDto == null)
            {
                return null;
            }

            var language = string.IsNullOrEmpty(cartDto.LanguageCode)
                ? Language.InvariantLanguage
                : new Language(cartDto.LanguageCode);

            return cartDto.ToShoppingCart(currency, language, new User
            {
                Id = userId
            });
        }

        //public virtual async Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria)
        //{
        //    if (criteria == null)
        //    {
        //        throw new ArgumentNullException(nameof(criteria));
        //    }

        //    var resultDto = await _cartApi.SearchShoppingCartAsync(criteria.ToSearchCriteriaDto());
        //    var result = new List<ShoppingCart>();
        //    foreach (var cartDto in resultDto.Results)
        //    {
        //        var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
        //        var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
        //        var user = await _userManager.FindByIdAsync(cartDto.CustomerId) ?? criteria.Customer;
        //        var cart = cartDto.ToShoppingCart(currency, language, user);
        //        result.Add(cart);
        //    }

        //    return new StaticPagedList<ShoppingCart>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);

        //}
    }
}
