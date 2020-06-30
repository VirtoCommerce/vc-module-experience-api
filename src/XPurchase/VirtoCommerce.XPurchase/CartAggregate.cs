using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.ShippingModule.Core.Model.Search;
using VirtoCommerce.ShippingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;
using Store = VirtoCommerce.StoreModule.Core.Model.Store;

namespace VirtoCommerce.XPurchase
{
    public class CartAggregate : Entity, IAggregateRoot
    {
        private readonly ICartProductService _cartProductService;
        private readonly ICurrencyService _currencyService;
        private readonly IMarketingPromoEvaluator _marketingEvaluator;
        private readonly IPaymentMethodsSearchService _paymentMethodsSearchService;
        private readonly IShippingMethodsSearchService _shippingMethodsSearchService;
        private readonly IShoppingCartTotalsCalculator _cartTotalsCalculator;
        private readonly IStoreService _storeService;
        private readonly ITaxProviderSearchService _taxProviderSearchService;

        private readonly IMapper _mapper;

        public CartAggregate(
            ICartProductService cartProductService,
            ICurrencyService currencyService,
            IMarketingPromoEvaluator marketingEvaluator,
            IPaymentMethodsSearchService paymentMethodsSearchService,
            IShippingMethodsSearchService shippingMethodsSearchService,
            IShoppingCartTotalsCalculator cartTotalsCalculator,
            IStoreService storeService,
            ITaxProviderSearchService taxProviderSearchService,
            IMapper mapper
            )
        {
            _cartProductService = cartProductService;
            _cartTotalsCalculator = cartTotalsCalculator;
            _currencyService = currencyService;
            _marketingEvaluator = marketingEvaluator;
            _paymentMethodsSearchService = paymentMethodsSearchService;
            _shippingMethodsSearchService = shippingMethodsSearchService;
            _storeService = storeService;
            _taxProviderSearchService = taxProviderSearchService;
            _mapper = mapper;
        }

        public Store Store { get; protected set; }
        public Currency Currency { get; protected set; }

        public virtual ShoppingCart Cart { get; protected set; }

        public virtual IDictionary<string, CartProduct> CartProductsDict { get; protected set; } = new Dictionary<string, CartProduct>().WithDefaultValue(null);

        /// <summary>
        /// Contains a new of validation rule set that will be executed each time the basket is changed.
        /// FluentValidation RuleSets allow you to group validation rules together which can be executed together as a group. You can set exists rule set name to evaluate default.
        /// <see cref="CartValidator"/>
        /// </summary>
        public string ValidationRuleSet { get; set; } = "default,strict";

        public bool IsValid => ValidationErrors.Any();
        public IList<ValidationFailure> ValidationErrors { get; set; } = new List<ValidationFailure>();

        public virtual async Task<CartAggregate> TakeCartAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            Id = cart.Id;

            Cart = cart;

            var allCurrencies = await _currencyService.GetAllCurrenciesAsync();

            var cartCurrency = allCurrencies.FirstOrDefault(x => x.Code.EqualsInvariant(cart.Currency));
            if (cartCurrency == null)
            {
                throw new OperationCanceledException($"cart currency {cart.Currency} is not registered in the system");
            }
            //Clone  currency with cart language
            //TODO: Use language from request context
            Currency = new Currency(new Language(cart.LanguageCode), cartCurrency.Code, cartCurrency.Name, cartCurrency.Symbol, cartCurrency.ExchangeRate)
            {
                CustomFormatting = cartCurrency.CustomFormatting
            };

            Store = await _storeService.GetByIdAsync(cart.StoreId);

            //Load products for all cart items
            if (!cart.Items.IsNullOrEmpty())
            {
                var cartItems = cart.Items.Select(x => x.ProductId).ToArray();

                var cartProducts = await _cartProductService.GetCartProductsByIdsAsync(this, cartItems);

                CartProductsDict = cartProducts.ToDictionary(x => x.Id).WithDefaultValue(null);
            }

            await RecalculateAsync();

            return this;
        }

        public virtual Task<CartAggregate> UpdateCartComment(string comment)
        {
            EnsureCartExists();

            Cart.Comment = comment;

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> AddItemAsync(NewCartItem newCartItem)
        {
            EnsureCartExists();

            if (newCartItem == null)
            {
                throw new ArgumentNullException(nameof(newCartItem));
            }

            // TODO: no behavior for newCartItem.ProductId == null before using _cartProductService.GetCartProductsByIdsAsync

            //Load actual cart product with all prices and inventories  for newly added item
            var cartProducts = await _cartProductService.GetCartProductsByIdsAsync(this, new[] { newCartItem.ProductId });

            newCartItem.CartProduct = cartProducts.FirstOrDefault();

            await new NewCartItemValidator().ValidateAndThrowAsync(newCartItem, ruleSet: ValidationRuleSet);

            var lineItem = _mapper.Map<LineItem>(newCartItem.CartProduct);

            if (newCartItem.Price != null)
            {
                lineItem.ListPrice = newCartItem.Price.Value;
                lineItem.SalePrice = newCartItem.Price.Value;
            }

            if (!string.IsNullOrEmpty(newCartItem.Comment))
            {
                lineItem.Note = newCartItem.Comment;
            }

            if (!newCartItem.DynamicProperties.IsNullOrEmpty())
            {
                lineItem.DynamicProperties = newCartItem.DynamicProperties.Select(x => new DynamicObjectProperty
                {
                    Name = x.Key,
                    Values = new[] { new DynamicPropertyObjectValue { Value = x.Value } }
                }).ToList();
            }

            await AddLineItemAsync(lineItem);

            await RecalculateAsync();

            return this;
        }

        public virtual async Task<CartAggregate> ChangeItemPriceAsync(PriceAdjustment priceAdjustment)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == priceAdjustment.LineItemId);
            if (lineItem != null)
            {
                await new ChangeCartItemPriceValidator(this).ValidateAndThrowAsync(priceAdjustment, ruleSet: ValidationRuleSet);
                lineItem.ListPrice = priceAdjustment.NewPrice;
                lineItem.SalePrice = priceAdjustment.NewPrice;
                await RecalculateAsync();
            }

            return this;
        }

        public virtual async Task<CartAggregate> ChangeItemQuantityAsync(ItemQtyAdjustment qtyAdjustment)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == qtyAdjustment.LineItemId);

            if (lineItem != null && !lineItem.IsReadOnly)
            {
                var lineItemProduct = CartProductsDict[lineItem.ProductId];
                if (lineItemProduct != null)
                {
                    var salePrice = lineItemProduct.Price.GetTierPrice(qtyAdjustment.NewQuantity).Price;
                    if (salePrice != 0)
                    {
                        lineItem.SalePrice = salePrice.Amount;
                    }
                    //List price should be always greater ot equals sale price because it may cause incorrect totals calculation
                    if (lineItem.ListPrice < lineItem.SalePrice)
                    {
                        lineItem.ListPrice = lineItem.SalePrice;
                    }
                }
                if (qtyAdjustment.NewQuantity > 0)
                {
                    lineItem.Quantity = qtyAdjustment.NewQuantity;
                }
                else
                {
                    Cart.Items.Remove(lineItem);
                }
            }
            await RecalculateAsync();
            return this;
        }

        public virtual Task<CartAggregate> ChangeItemCommentAsync(NewItemComment newItemComment)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == newItemComment.LineItemId);
            if (lineItem != null)
            {
                lineItem.Note = newItemComment.Comment;
            }

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> RemoveItemAsync(string lineItemId)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                Cart.Items.Remove(lineItem);
                await RecalculateAsync();
            }

            return this;
        }

        public virtual async Task<CartAggregate> AddCouponAsync(string couponCode)
        {
            EnsureCartExists();
            if (!Cart.Coupons.Any(c => c.EqualsInvariant(couponCode)))
            {
                Cart.Coupons.Add(couponCode);
                await RecalculateAsync();
            }
            return this;
        }

        public virtual async Task<CartAggregate> RemoveCouponAsync(string couponCode = null)
        {
            EnsureCartExists();
            if (string.IsNullOrEmpty(couponCode))
            {
                Cart.Coupons.Clear();
            }
            else
            {
                Cart.Coupons.Remove(Cart.Coupons.FirstOrDefault(c => c.EqualsInvariant(couponCode)));
            }
            await RecalculateAsync();
            return this;
        }

        public virtual async Task<CartAggregate> ClearAsync()
        {
            EnsureCartExists();

            Cart.Items.Clear();
            await RecalculateAsync();
            return this;
        }

        public virtual async Task<CartAggregate> AddOrUpdateShipmentAsync(Shipment shipment)
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

            if (!string.IsNullOrEmpty(shipment.ShipmentMethodCode) && !Cart.IsTransient())
            {
                var availableShippingRates = await GetAvailableShippingRatesAsync();
                var shippingMethod = availableShippingRates.FirstOrDefault(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShippingMethod.Code) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                if (shippingMethod == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown shipment method: {0} with option: {1}", shipment.ShipmentMethodCode, shipment.ShipmentMethodOption));
                }
                shipment.Price = shippingMethod.Rate;
                shipment.DiscountAmount = shippingMethod.DiscountAmount;
                //TODO:
                //shipment.TaxType = shippingMethod.TaxType;
            }

            await RecalculateAsync();
            return this;
        }

        public virtual async Task<CartAggregate> RemoveShipmentAsync(string shipmentId)
        {
            EnsureCartExists();

            var shipment = Cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                Cart.Shipments.Remove(shipment);
            }
            await RecalculateAsync();
            return this;
        }

        public virtual async Task<CartAggregate> AddOrUpdatePaymentAsync(Payment payment)
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
                    throw new InvalidOperationException("Unknown payment method " + payment.PaymentGatewayCode);
                }
            }
            await RecalculateAsync();
            return this;
        }

        public virtual async Task<CartAggregate> MergeWithCartAsync(ShoppingCart cart)
        {
            EnsureCartExists();

            //Reset primary keys for all aggregated entities before merge
            //To prevent insertions same Ids for target cart
            //exclude user because it might be the current one
            var entities = cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities)
            {
                entity.Id = null;
            }

            foreach (var lineItem in cart.Items)
            {
                await AddLineItemAsync(lineItem);
            }

            foreach (var coupon in cart.Coupons)
            {
                await AddCouponAsync(coupon);
            }

            foreach (var shipment in cart.Shipments)
            {
                await AddOrUpdateShipmentAsync(shipment);
            }

            foreach (var payment in cart.Payments)
            {
                await AddOrUpdatePaymentAsync(payment);
            }
            await RecalculateAsync();
            return this;
        }

        //TODO: Remove code duplication and move out from here to the service or query
        public virtual async Task<IEnumerable<ShippingRate>> GetAvailableShippingRatesAsync()
        {
            EnsureCartExists();

            //Request available shipping rates
            var shippingEvaluationContext = new ShippingEvaluationContext(Cart);

            var criteria = new ShippingMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = Cart.StoreId
            };

            var activeAvailableShippingMethods = (await _shippingMethodsSearchService.SearchShippingMethodsAsync(criteria)).Results;

            var availableShippingRates = activeAvailableShippingMethods
                .SelectMany(x => x.CalculateRates(shippingEvaluationContext))
                .Where(x => x.ShippingMethod == null || x.ShippingMethod.IsActive)
                .ToArray();

            if (availableShippingRates.IsNullOrEmpty())
            {
                return Enumerable.Empty<ShippingRate>();
            }

            //Evaluate promotions cart and apply rewards for available shipping methods
            var evalContext = _mapper.Map<PromotionEvaluationContext>(this);
            var promoResult = await _marketingEvaluator.EvaluatePromotionAsync(evalContext);
            foreach (var shippingRate in availableShippingRates)
            {
                shippingRate.ApplyRewards(promoResult.Rewards);
            }

            var taxProvider = await GetActiveTaxProviderAsync();
            if (taxProvider != null)
            {
                var taxEvalContext = _mapper.Map<TaxEvaluationContext>(this);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(availableShippingRates.SelectMany(x => _mapper.Map<IEnumerable<TaxLine>>(x)));
                var taxRates = taxProvider.CalculateRates(taxEvalContext);
                foreach (var shippingRate in availableShippingRates)
                {
                    shippingRate.ApplyTaxRates(taxRates);
                }
            }

            return availableShippingRates;
        }

        //TODO: Remove code duplication and move out from here to the service or query
        public virtual async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            EnsureCartExists();

            var criteria = new PaymentMethodsSearchCriteria
            {
                IsActive = true,
                Take = int.MaxValue,
                StoreId = Cart.StoreId,
            };

            var result = await _paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            if (result.Results.IsNullOrEmpty())
            {
                return Enumerable.Empty<PaymentMethod>();
            }

            var evalContext = _mapper.Map<PromotionEvaluationContext>(this);
            var promoResult = await _marketingEvaluator.EvaluatePromotionAsync(evalContext);

            foreach (var paymentMethod in result.Results)
            {
                paymentMethod.ApplyRewards(promoResult.Rewards);
            }

            //Evaluate taxes for available payments
            var taxProvider = await GetActiveTaxProviderAsync();
            if (taxProvider != null)
            {
                var taxEvalContext = _mapper.Map<TaxEvaluationContext>(this);
                taxEvalContext.Lines.Clear();
                taxEvalContext.Lines.AddRange(result.Results.SelectMany(x => _mapper.Map<IEnumerable<TaxLine>>(x)));
                var taxRates = taxProvider.CalculateRates(taxEvalContext);
                foreach (var paymentMethod in result.Results)
                {
                    paymentMethod.ApplyTaxRates(taxRates);
                }
            }

            return result.Results;
        }

        public async Task<CartAggregate> ValidateAsync()
        {
            EnsureCartExists();
            var result = await new CartValidator().ValidateAsync(this, ruleSet: ValidationRuleSet);
            if (!result.IsValid)
            {
                ValidationErrors.AddRange(result.Errors);
            }
            return this;
        }

        public async Task<bool> ValidateCouponAsync(CartCoupon coupon)
        {
            EnsureCartExists();

            var promotionResult = await EvaluatePromotionsAsync();
            if (promotionResult.Rewards == null)
            {
                return false;
            }

            var validCoupon = promotionResult.Rewards.FirstOrDefault(x => x.IsValid && x.Coupon == coupon.Code);

            return validCoupon != null;
        }

        protected virtual async Task<PromotionResult> EvaluatePromotionsAsync()
        {
            EnsureCartExists();

            if (Cart.Items.IsNullOrEmpty() || !Cart.Items.Any(i => i.IsReadOnly))
            {
                return null;
            }

            var evalContext = _mapper.Map<PromotionEvaluationContext>(this);

            var promotionResult = await _marketingEvaluator.EvaluatePromotionAsync(evalContext);

            return promotionResult;
        }

        protected async Task<CartAggregate> EvaluateTaxesAsync()
        {
            EnsureCartExists();

            var taxProvider = await GetActiveTaxProviderAsync();
            if (taxProvider != null)
            {
                var taxEvalContext = _mapper.Map<TaxEvaluationContext>(this);
                var taxRates = taxProvider.CalculateRates(taxEvalContext);
                Cart.ApplyTaxRates(taxRates);
            }

            return this;
        }

        public virtual async Task<CartAggregate> RecalculateAsync()
        {
            EnsureCartExists();
            await EvaluatePromotionsAsync();
            await EvaluateTaxesAsync();
            _cartTotalsCalculator.CalculateTotals(Cart);
            return this;
        }

        protected virtual Task<CartAggregate> RemoveExistingPaymentAsync(Payment payment)
        {
            if (payment != null)
            {
                var existingPayment = !payment.IsTransient() ? Cart.Payments.FirstOrDefault(s => s.Id == payment.Id) : null;
                if (existingPayment != null)
                {
                    Cart.Payments.Remove(existingPayment);
                }
            }

            return Task.FromResult(this);
        }

        protected virtual Task<CartAggregate> RemoveExistingShipmentAsync(Shipment shipment)
        {
            if (shipment != null)
            {
                var existShipment = !shipment.IsTransient() ? Cart.Shipments.FirstOrDefault(s => s.Id == shipment.Id) : null;
                if (existShipment != null)
                {
                    Cart.Shipments.Remove(existShipment);
                }
            }
            return Task.FromResult(this);
        }

        protected virtual Task<CartAggregate> ChangeItemQuantityAsync(LineItem lineItem, int quantity)
        {
            if (lineItem != null && !lineItem.IsReadOnly)
            {
                var cartProduct = CartProductsDict[lineItem.ProductId];
                if (cartProduct != null)
                {
                    var salePrice = cartProduct.Price.GetTierPrice(quantity).Price;
                    if (salePrice != 0)
                    {
                        lineItem.SalePrice = salePrice.Amount;
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

            return Task.FromResult(this);
        }

        protected virtual async Task<CartAggregate> AddLineItemAsync(LineItem lineItem)
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

            return this;
        }

        protected virtual void EnsureCartExists()
        {
            if (Cart == null)
            {
                throw new OperationCanceledException("Cart not loaded.");
            }
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

            return payments.Results.OrderBy(x => x.Priority).ToList();
        }

        protected async Task<TaxProvider> GetActiveTaxProviderAsync()
        {
            //TODO:
            //if (!context.StoreTaxCalculationEnabled)
            //{
            //    return;
            //}

            var storeTaxProviders = await _taxProviderSearchService.SearchTaxProvidersAsync(new TaxProviderSearchCriteria
            {
                StoreIds = new[] { Cart.StoreId }
            });

            return storeTaxProviders?.Results.FirstOrDefault(x => x.IsActive);
        }
    }
}
