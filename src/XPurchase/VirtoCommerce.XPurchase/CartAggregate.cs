using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Validators;
using Store = VirtoCommerce.StoreModule.Core.Model.Store;

namespace VirtoCommerce.XPurchase
{
    public class CartAggregate : Entity, IAggregateRoot, ICloneable
    {
        private readonly IMarketingPromoEvaluator _marketingEvaluator;
        private readonly IShoppingCartTotalsCalculator _cartTotalsCalculator;
        private readonly ITaxProviderSearchService _taxProviderSearchService;

        private readonly IMapper _mapper;

        public CartAggregate(
            IMarketingPromoEvaluator marketingEvaluator,
            IShoppingCartTotalsCalculator cartTotalsCalculator,
            ITaxProviderSearchService taxProviderSearchService,
            IMapper mapper
            )
        {
            _cartTotalsCalculator = cartTotalsCalculator;
            _marketingEvaluator = marketingEvaluator;
            _taxProviderSearchService = taxProviderSearchService;
            _mapper = mapper;
        }

        public Store Store { get; protected set; }
        public Currency Currency { get; protected set; }
        public Member Member { get; protected set; }

        public IEnumerable<CartCoupon> Coupons
        {
            get
            {
                //TODO: refactor to be more performance
                var allAppliedCoupons = Cart.GetFlatObjectsListWithInterface<IHasDiscounts>()
                                            .SelectMany(x => x.Discounts ?? Array.Empty<Discount>())
                                            .Where(x => !string.IsNullOrEmpty(x.Coupon))
                                            .Select(x => x.Coupon)
                                            .Distinct()
                                            .ToList();

                foreach (var coupon in Cart.Coupons)
                {
                    var cartCoupon = new CartCoupon
                    {
                        Code = coupon,
                        IsAppliedSuccessfully = allAppliedCoupons.Contains(coupon)
                    };
                    yield return cartCoupon;
                }
            }
        }

        public ShoppingCart Cart { get; protected set; }

        /// <summary>
        /// Represents the dictionary of all CartProducts data for each  existing cart line item
        /// </summary>
        public IDictionary<string, CartProduct> CartProducts { get; protected set; } = new Dictionary<string, CartProduct>().WithDefaultValue(null);

        /// <summary>
        /// Contains a new of validation rule set that will be executed each time the basket is changed.
        /// FluentValidation RuleSets allow you to group validation rules together which can be executed together as a group. You can set exists rule set name to evaluate default.
        /// <see cref="CartValidator"/>
        /// </summary>
        public string ValidationRuleSet { get; set; } = "default,strict";

        public bool IsValid => !ValidationErrors.Any();
        public IList<ValidationFailure> ValidationErrors { get; protected set; } = new List<ValidationFailure>();

        public virtual CartAggregate GrabCart(ShoppingCart cart, Store store, Member member, Currency currency)
        {
            Id = cart.Id;
            Cart = cart;
            Member = member;
            Currency = currency;
            Store = store;
            Cart.IsAnonymous = member == null;
            //TODO: Need to check what member.Name contains name for all derived member types such as contact etc.
            Cart.CustomerName = member?.Name ?? "Anonymous";

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

            var validationResult = await new NewCartItemValidator().ValidateAsync(newCartItem, ruleSet: ValidationRuleSet);
            if (!validationResult.IsValid)
            {
                ValidationErrors.AddRange(validationResult.Errors);
            }

            if (newCartItem.CartProduct != null)
            {
                var lineItem = _mapper.Map<LineItem>(newCartItem.CartProduct);
                lineItem.Quantity = newCartItem.Quantity;

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

                CartProducts[newCartItem.CartProduct.Id] = newCartItem.CartProduct;
                await InnerAddLineItemAsync(lineItem, newCartItem.CartProduct);
            }

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
            }

            return this;
        }

        public virtual async Task<CartAggregate> ChangeItemQuantityAsync(ItemQtyAdjustment qtyAdjustment)
        {
            EnsureCartExists();

            var validationResult = await new ItemQtyAdjustmentValidator(this).ValidateAsync(qtyAdjustment, ruleSet: ValidationRuleSet);
            if (!validationResult.IsValid)
            {
                ValidationErrors.AddRange(validationResult.Errors);
            }

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == qtyAdjustment.LineItemId);

            if (lineItem != null)
            {
                var salePrice = qtyAdjustment.CartProduct.Price.GetTierPrice(qtyAdjustment.NewQuantity).Price;
                if (salePrice != 0)
                {
                    lineItem.SalePrice = salePrice.Amount;
                }

                //List price should be always greater or equals sale price because it may cause incorrect totals calculation
                lineItem.ListPrice = lineItem.ListPrice < lineItem.SalePrice
                    ? lineItem.SalePrice
                    : lineItem.ListPrice;

                lineItem.Quantity = qtyAdjustment.NewQuantity;
            }

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

        public virtual Task<CartAggregate> RemoveItemAsync(string lineItemId)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                Cart.Items.Remove(lineItem);
            }

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> AddCouponAsync(string couponCode)
        {
            EnsureCartExists();

            if (!Cart.Coupons.Any(c => c.EqualsInvariant(couponCode)))
            {
                Cart.Coupons.Add(couponCode);
            }

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> RemoveCouponAsync(string couponCode = null)
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
            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> ClearAsync()
        {
            EnsureCartExists();

            Cart.Items.Clear();

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> AddShipmentAsync(Shipment shipment, IEnumerable<ShippingRate> availRates)
        {
            EnsureCartExists();

            await new CartShipmentValidator(availRates).ValidateAndThrowAsync(shipment, ruleSet: ValidationRuleSet);

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
                var shippingMethod = availRates.First(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShippingMethod.Code) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                shipment.Price = shippingMethod.Rate;
                shipment.DiscountAmount = shippingMethod.DiscountAmount;
                //TODO:
                //shipment.TaxType = shippingMethod.TaxType;
            }
            return this;
        }

        public virtual Task<CartAggregate> RemoveShipmentAsync(string shipmentId)
        {
            EnsureCartExists();

            var shipment = Cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                Cart.Shipments.Remove(shipment);
            }
            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> AddOrUpdateCartAddress(CartModule.Core.Model.Address address)
        {
            EnsureCartExists();
            //Remove existing address 
            Cart.Addresses.Remove(address);
            Cart.Addresses.Add(address);

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> RemoveCartAddress(CartModule.Core.Model.Address address)
        {
            EnsureCartExists();
            //Remove existing address 
            Cart.Addresses.Remove(address);

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> AddPaymentAsync(Payment payment, IEnumerable<PaymentMethod> availPaymentMethods)
        {
            EnsureCartExists();

            await new CartPaymentValidator(availPaymentMethods).ValidateAndThrowAsync(payment, ruleSet: ValidationRuleSet);

            if (payment.Currency == null)
            {
                payment.Currency = Cart.Currency;
            }
            await RemoveExistingPaymentAsync(payment);
            if (payment.BillingAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address
                payment.BillingAddress.Key = null;
            }

            Cart.Payments.Add(payment);

            return this;
        }

        public virtual async Task<CartAggregate> MergeWithCartAsync(CartAggregate otherCart)
        {
            EnsureCartExists();

            //Reset primary keys for all aggregated entities before merge
            //To prevent insertions same Ids for target cart
            //exclude user because it might be the current one
            var entities = otherCart.Cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities)
            {
                entity.Id = null;
            }

            foreach (var lineItem in otherCart.Cart.Items.ToList())
            {
                await InnerAddLineItemAsync(lineItem, otherCart.CartProducts[lineItem.ProductId]);
            }

            foreach (var coupon in otherCart.Cart.Coupons.ToList())
            {
                await AddCouponAsync(coupon);
            }

            foreach (var shipment in otherCart.Cart.Shipments.ToList())
            {
                //Skip validation, do not pass avail methods
                await AddShipmentAsync(shipment, null);
            }

            foreach (var payment in otherCart.Cart.Payments.ToList())
            {
                //Skip validation, do not pass avail methods
                await AddPaymentAsync(payment, null);
            }

            return this;
        }

        public async Task<IList<ValidationFailure>> ValidateAsync(CartValidationContext validationContext)
        {
            EnsureCartExists();

            ValidationErrors.Clear();
            var result = await new CartValidator(validationContext).ValidateAsync(this, ruleSet: ValidationRuleSet);
            if (!result.IsValid)
            {
                ValidationErrors.AddRange(result.Errors);
            }
            return result.Errors;
        }

        public async Task<bool> ValidateCouponAsync(string coupon)
        {
            EnsureCartExists();

            var promotionResult = await EvaluatePromotionsAsync();
            if (promotionResult.Rewards == null)
            {
                return false;
            }

            var validCoupon = promotionResult.Rewards.FirstOrDefault(x => x.IsValid && x.Coupon == coupon);

            return validCoupon != null;
        }

        public virtual async Task<PromotionResult> EvaluatePromotionsAsync()
        {
            EnsureCartExists();

            var promotionResult = new PromotionResult();
            if (!Cart.Items.IsNullOrEmpty() && !Cart.Items.Any(i => i.IsReadOnly))
            {
                var evalContext = _mapper.Map<PromotionEvaluationContext>(this);
                promotionResult = await EvaluatePromotionsAsync(evalContext);
            }

            return promotionResult;
        }

        public virtual async Task<PromotionResult> EvaluatePromotionsAsync(PromotionEvaluationContext evalContext)
        {
            return await _marketingEvaluator.EvaluatePromotionAsync(evalContext);
        }

        protected async Task<IEnumerable<TaxRate>> EvaluateTaxesAsync()
        {
            EnsureCartExists();
            var result = Enumerable.Empty<TaxRate>();
            var taxProvider = await GetActiveTaxProviderAsync();
            if (taxProvider != null)
            {
                var taxEvalContext = _mapper.Map<TaxEvaluationContext>(this);
                result = taxProvider.CalculateRates(taxEvalContext);
            }
            return result;
        }

        public virtual async Task<CartAggregate> RecalculateAsync()
        {
            EnsureCartExists();

            var promotionEvalResult = await EvaluatePromotionsAsync();
            Cart.ApplyRewards(promotionEvalResult.Rewards);

            var taxRates = await EvaluateTaxesAsync();
            Cart.ApplyTaxRates(taxRates);

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
                // Get unique shipment from shipments by code/option pair or by id
                var existShipment = Cart.Shipments.FirstOrDefault(s => !shipment.IsTransient() && s.Id == shipment.Id);

                if (existShipment != null)
                {
                    Cart.Shipments.Remove(existShipment);
                }
            }

            return Task.FromResult(this);
        }

        protected virtual Task<CartAggregate> InnerChangeItemQuantityAsync(LineItem lineItem, int quantity, CartProduct product = null)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException(nameof(lineItem));
            }

            if (!lineItem.IsReadOnly && product != null)
            {
                var salePrice = product.Price.GetTierPrice(quantity).Price;
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
            return Task.FromResult(this);
        }

        protected virtual async Task<CartAggregate> InnerAddLineItemAsync(LineItem lineItem, CartProduct product = null)
        {
            var existingLineItem = Cart.Items.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                await InnerChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity), product);
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

        #region ICloneable

        public virtual object Clone()
        {
            var result = MemberwiseClone() as CartAggregate;

            result.Cart = Cart?.Clone() as ShoppingCart;
            result.CartProducts = CartProducts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Clone() as CartProduct);
            result.Currency = Currency.Clone() as Currency;
            result.Member = Member.Clone() as Member;
            result.Store = Store.Clone() as Store;

            return result;
        }

        #endregion ICloneable
    }
}
