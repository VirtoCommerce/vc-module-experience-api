using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.ShippingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model.Search;
using VirtoCommerce.TaxModule.Core.Services;
using VirtoCommerce.XPurchase.Extensions;
using VirtoCommerce.XPurchase.Services;
using VirtoCommerce.XPurchase.Validators;
using Store = VirtoCommerce.StoreModule.Core.Model.Store;
using StoreSetting = VirtoCommerce.StoreModule.Core.ModuleConstants.Settings.General;

namespace VirtoCommerce.XPurchase
{
    [DebuggerDisplay("CartId = {Cart.Id}")]
    public class CartAggregate : Entity, IAggregateRoot, ICloneable
    {
        private readonly IMarketingPromoEvaluator _marketingEvaluator;
        private readonly IShoppingCartTotalsCalculator _cartTotalsCalculator;
        private readonly ITaxProviderSearchService _taxProviderSearchService;
        private readonly ICartProductService _cartProductService;
        private readonly IDynamicPropertyUpdaterService _dynamicPropertyUpdaterService;
        private readonly IMemberOrdersService _memberOrdersService;
        private readonly IMapper _mapper;

        private bool? _isFirstTimeBuyer;

        public CartAggregate(
            IMarketingPromoEvaluator marketingEvaluator,
            IShoppingCartTotalsCalculator cartTotalsCalculator,
            ITaxProviderSearchService taxProviderSearchService,
            ICartProductService cartProductService,
            IDynamicPropertyUpdaterService dynamicPropertyUpdaterService,
            IMapper mapper,
            IMemberOrdersService memberOrdersService)
        {
            _cartTotalsCalculator = cartTotalsCalculator;
            _marketingEvaluator = marketingEvaluator;
            _taxProviderSearchService = taxProviderSearchService;
            _cartProductService = cartProductService;
            _dynamicPropertyUpdaterService = dynamicPropertyUpdaterService;
            _mapper = mapper;
            _memberOrdersService = memberOrdersService;
        }

        public Store Store { get; protected set; }
        public Currency Currency { get; protected set; }
        public Member Member { get; protected set; }

        public IEnumerable<CartCoupon> Coupons
        {
            get
            {
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
        public IEnumerable<LineItem> GiftItems => Cart?.Items.Where(x => x.IsGift) ?? Enumerable.Empty<LineItem>();
        public IEnumerable<LineItem> LineItems => Cart?.Items.Where(x => !x.IsGift) ?? Enumerable.Empty<LineItem>();
        public IEnumerable<LineItem> SelectedLineItems => LineItems.Where(x => x.SelectedForCheckout);

        /// <summary>
        /// Represents the dictionary of all CartProducts data for each  existing cart line item
        /// </summary>
        public IDictionary<string, CartProduct> CartProducts { get; protected set; } = new Dictionary<string, CartProduct>().WithDefaultValue(null);

        /// <summary>
        /// Contains a new of validation rule set that will be executed each time the basket is changed.
        /// FluentValidation RuleSets allow you to group validation rules together which can be executed together as a group. You can set exists rule set name to evaluate default.
        /// <see cref="CartValidator"/>
        /// </summary>
        public string[] ValidationRuleSet { get; set; } = { "default", "strict" };

        public bool IsValid => !ValidationErrors.Any();
        public IList<ValidationFailure> ValidationErrors { get; protected set; } = new List<ValidationFailure>();
        public bool IsValidated { get; private set; }

        public bool IsFirstBuyer
        {
            get
            {
                if (_isFirstTimeBuyer != null)
                {
                    return _isFirstTimeBuyer.Value;
                }

                _isFirstTimeBuyer = Cart.IsAnonymous || _memberOrdersService.IsFirstTimeBuyer(Cart.CustomerId);
                return _isFirstTimeBuyer.Value;
            }
        }

        public IList<ValidationFailure> ValidationWarnings { get; protected set; } = new List<ValidationFailure>();

        public virtual string Scope
        {
            get
            {
                return string.IsNullOrEmpty(Cart.OrganizationId) ? XPurchaseConstants.PrivateScope : XPurchaseConstants.OrganizationScope;
            }
        }

        public virtual CartAggregate GrabCart(ShoppingCart cart, Store store, Member member, Currency currency)
        {
            Id = cart.Id;
            Cart = cart;
            Member = member;
            Currency = currency;
            Store = store;
            Cart.IsAnonymous = member == null;
            Cart.CustomerName = member?.Name ?? "Anonymous";
            Cart.Items ??= new List<LineItem>();

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

            var validationResult = await AbstractTypeFactory<NewCartItemValidator>.TryCreateInstance().ValidateAsync(newCartItem, options => options.IncludeRuleSets(ValidationRuleSet));
            if (!validationResult.IsValid)
            {
                ValidationErrors.AddRange(validationResult.Errors);
            }
            else if (newCartItem.CartProduct != null)
            {
                if (newCartItem.IsWishlist && newCartItem.CartProduct.Price == null)
                {
                    newCartItem.CartProduct.Price = new ProductPrice(Currency);
                }

                var lineItem = _mapper.Map<LineItem>(newCartItem.CartProduct);

                lineItem.Quantity = newCartItem.Quantity;

                if (newCartItem.Price != null)
                {
                    lineItem.ListPrice = newCartItem.Price.Value;
                    lineItem.SalePrice = newCartItem.Price.Value;
                }
                else
                {
                    SetLineItemTierPrice(newCartItem.CartProduct.Price, newCartItem.Quantity, lineItem);
                }

                if (!string.IsNullOrEmpty(newCartItem.Comment))
                {
                    lineItem.Note = newCartItem.Comment;
                }

                CartProducts[newCartItem.CartProduct.Id] = newCartItem.CartProduct;
                await SetItemFulfillmentCenterAsync(lineItem, newCartItem.CartProduct);
                await UpdateVendor(lineItem, newCartItem.CartProduct);
                await InnerAddLineItemAsync(lineItem, newCartItem.CartProduct, newCartItem.DynamicProperties);
            }

            return this;
        }

        public virtual async Task<CartAggregate> AddItemsAsync(ICollection<NewCartItem> newCartItems)
        {
            EnsureCartExists();

            var productIds = newCartItems.Select(x => x.ProductId).Distinct().ToArray();

            var productsByIds =
                (await _cartProductService.GetCartProductsByIdsAsync(this, productIds))
                .ToDictionary(x => x.Id);

            foreach (var item in newCartItems)
            {
                if (productsByIds.TryGetValue(item.ProductId, out var product))
                {
                    await AddItemAsync(new NewCartItem(item.ProductId, item.Quantity)
                    {
                        Comment = item.Comment,
                        DynamicProperties = item.DynamicProperties,
                        Price = item.Price,
                        IsWishlist = item.IsWishlist,
                        CartProduct = product,
                    });
                }
            }

            return this;
        }

        public virtual Task<CartAggregate> AddGiftItemsAsync(IReadOnlyCollection<string> giftIds, IReadOnlyCollection<GiftItem> availableGifts)
        {
            EnsureCartExists();

            if (!giftIds.IsNullOrEmpty())
            {
                foreach (var giftId in giftIds)
                {
                    var availableGift = availableGifts.FirstOrDefault(x => x.Id == giftId);
                    if (availableGift == null)
                    {
                        // ignore the gift, if it's not in available gifts list
                        continue;
                    }

                    var giftItem = GiftItems.FirstOrDefault(x => x.EqualsReward(availableGift));
                    if (giftItem == null)
                    {
                        giftItem = _mapper.Map<LineItem>(availableGift);
                        giftItem.Id = null;
                        giftItem.IsGift = true;
                        giftItem.CatalogId ??= "";
                        giftItem.ProductId ??= "";
                        giftItem.Sku ??= "";
                        giftItem.Currency = Currency.Code;
                        Cart.Items.Add(giftItem);
                    }

                    giftItem.IsRejected = false;
                }
            }

            return Task.FromResult(this);
        }

        public virtual CartAggregate RejectCartItems(IReadOnlyCollection<string> cartItemIds)
        {
            EnsureCartExists();

            if (cartItemIds.IsNullOrEmpty())
            {
                return this;
            }

            foreach (var cartItemId in cartItemIds)
            {
                var giftItem = GiftItems.FirstOrDefault(x => x.Id == cartItemId);
                if (giftItem != null)
                {
                    RemoveItemAsync(giftItem.Id);
                }
            }

            return this;
        }

        public virtual async Task<CartAggregate> ChangeItemPriceAsync(PriceAdjustment priceAdjustment)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == priceAdjustment.LineItemId);
            if (lineItem != null)
            {
                await AbstractTypeFactory<ChangeCartItemPriceValidator>.TryCreateInstance().ValidateAsync(priceAdjustment, options => options.IncludeRuleSets(ValidationRuleSet).ThrowOnFailures());
                lineItem.ListPrice = priceAdjustment.NewPrice;
                lineItem.SalePrice = priceAdjustment.NewPrice;
            }

            return this;
        }

        public virtual async Task<CartAggregate> ChangeItemQuantityAsync(ItemQtyAdjustment qtyAdjustment)
        {
            EnsureCartExists();

            var validationResult = await AbstractTypeFactory<ItemQtyAdjustmentValidator>.TryCreateInstance().ValidateAsync(qtyAdjustment, options => options.IncludeRuleSets(ValidationRuleSet));
            if (!validationResult.IsValid)
            {
                ValidationErrors.AddRange(validationResult.Errors);
            }

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == qtyAdjustment.LineItemId);

            if (lineItem != null)
            {
                SetLineItemTierPrice(qtyAdjustment.CartProduct.Price, qtyAdjustment.NewQuantity, lineItem);

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

        public virtual Task<CartAggregate> ChangeItemsSelectedAsync(IList<string> lineItemIds, bool selectedForCheckout)
        {
            EnsureCartExists();

            foreach (var lineItemId in lineItemIds)
            {
                var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
                if (lineItem != null)
                {
                    lineItem.SelectedForCheckout = selectedForCheckout;
                }
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

        public virtual Task<CartAggregate> RemoveItemsAsync(string[] lineItemIds)
        {
            EnsureCartExists();

            var lineItems = Cart.Items.Where(x => lineItemIds.Contains(x.Id)).ToList();
            if (lineItems.Any())
            {
                lineItems.ForEach(x => Cart.Items.Remove(x));
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

            Cart.Comment = string.Empty;
            Cart.PurchaseOrderNumber = string.Empty;
            Cart.Shipments.Clear();
            Cart.Payments.Clear();
            Cart.Addresses.Clear();

            Cart.Coupons.Clear();
            Cart.Items.Clear();
            Cart.DynamicProperties?.Clear();

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> ChangePurchaseOrderNumber(string purchaseOrderNumber)
        {
            EnsureCartExists();

            Cart.PurchaseOrderNumber = purchaseOrderNumber;

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> AddShipmentAsync(Shipment shipment, IEnumerable<ShippingRate> availRates)
        {
            EnsureCartExists();

            var validationContext = new ShipmentValidationContext
            {
                Shipment = shipment,
                AvailShippingRates = availRates
            };
            await AbstractTypeFactory<CartShipmentValidator>.TryCreateInstance().ValidateAsync(validationContext, options => options.IncludeRuleSets(ValidationRuleSet).ThrowOnFailures());

            await RemoveExistingShipmentAsync(shipment);

            shipment.Currency = Cart.Currency;
            if (shipment.DeliveryAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address
                shipment.DeliveryAddress.Key = null;
            }
            Cart.Shipments.Add(shipment);

            if (availRates != null && !string.IsNullOrEmpty(shipment.ShipmentMethodCode) && !Cart.IsTransient())
            {
                var shippingMethod = availRates.First(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShippingMethod.Code) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                shipment.Price = shippingMethod.Rate;
                shipment.DiscountAmount = shippingMethod.DiscountAmount;
                //PT-5421: use new model for resolve taxable logic for ShippingRate/ShippingMethod
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
            var validationContext = new PaymentValidationContext
            {
                Payment = payment,
                AvailPaymentMethods = availPaymentMethods
            };
            await AbstractTypeFactory<CartPaymentValidator>.TryCreateInstance().ValidateAsync(validationContext, options => options.IncludeRuleSets(ValidationRuleSet).ThrowOnFailures());

            payment.Currency ??= Cart.Currency;
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

        public virtual Task<CartAggregate> AddOrUpdateCartAddressByTypeAsync(CartModule.Core.Model.Address address)
        {
            EnsureCartExists();

            //Reset address key because it can equal a customer address from profile and if not do that it may cause
            //address primary key duplication error for multiple carts with the same address
            address.Key = null;

            var existingAddress = Cart.Addresses.FirstOrDefault(x => x.AddressType == address.AddressType);

            if (existingAddress != null)
            {
                Cart.Addresses.Remove(existingAddress);
            }

            Cart.Addresses.Add(address);

            return Task.FromResult(this);
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

            await MergeLineItemsFromCartAsync(otherCart);
            await MergeCouponsFromCartAsync(otherCart);
            await MergeShipmentsFromCartAsync(otherCart);
            await MergePaymentsFromCartAsync(otherCart);
            return this;
        }

        protected virtual async Task MergeLineItemsFromCartAsync(CartAggregate otherCart)
        {
            foreach (var lineItem in otherCart.Cart.Items.ToList())
            {
                await InnerAddLineItemAsync(lineItem, otherCart.CartProducts[lineItem.ProductId]);
            }
        }

        protected virtual async Task MergeCouponsFromCartAsync(CartAggregate otherCart)
        {
            foreach (var coupon in otherCart.Cart.Coupons.ToList())
            {
                await AddCouponAsync(coupon);
            }
        }

        protected virtual async Task MergeShipmentsFromCartAsync(CartAggregate otherCart)
        {
            foreach (var shipment in otherCart.Cart.Shipments.ToList())
            {
                //Skip validation, do not pass avail methods
                await AddShipmentAsync(shipment, null);
            }
        }

        protected virtual async Task MergePaymentsFromCartAsync(CartAggregate otherCart)
        {
            foreach (var payment in otherCart.Cart.Payments.ToList())
            {
                //Skip validation, do not pass avail methods
                await AddPaymentAsync(payment, null);
            }
        }

        [Obsolete("Use a separate method with ruleSet parameter. One of or comma-divided combination of \"items\",\"shipments\",\"payments\"")]
        public virtual Task<IList<ValidationFailure>> ValidateAsync(CartValidationContext validationContext)
        {
            return ValidateAsync(validationContext, "default,items,shipments,payments");
        }

        public virtual async Task<IList<ValidationFailure>> ValidateAsync(CartValidationContext validationContext, string ruleSet)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }
            validationContext.CartAggregate = this;

            EnsureCartExists();
            var result = await AbstractTypeFactory<CartValidator>.TryCreateInstance().ValidateAsync(validationContext, options => options.IncludeRuleSets(ruleSet));
            if (!result.IsValid)
            {
                ValidationErrors.AddRange(result.Errors);
            }
            IsValidated = true;
            return result.Errors;
        }

        public virtual async Task<bool> ValidateCouponAsync(string coupon)
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
            if (!LineItems.IsNullOrEmpty() && !LineItems.Any(i => i.IsReadOnly))
            {
                var evalContext = _mapper.Map<PromotionEvaluationContext>(this);
                promotionResult = await EvaluatePromotionsAsync(evalContext);
            }

            return promotionResult;
        }

        public virtual Task<PromotionResult> EvaluatePromotionsAsync(PromotionEvaluationContext evalContext)
        {
            return _marketingEvaluator.EvaluatePromotionAsync(evalContext);
        }

        protected virtual async Task<IEnumerable<TaxRate>> EvaluateTaxesAsync()
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
            this.ApplyRewards(promotionEvalResult.Rewards);

            var taxRates = await EvaluateTaxesAsync();
            Cart.ApplyTaxRates(taxRates);

            _cartTotalsCalculator.CalculateTotals(Cart);
            return this;
        }

        public virtual Task<CartAggregate> SetItemFulfillmentCenterAsync(LineItem lineItem, CartProduct cartProduct)
        {
            lineItem.FulfillmentCenterId = cartProduct?.Inventory?.FulfillmentCenterId;
            lineItem.FulfillmentCenterName = cartProduct?.Inventory?.FulfillmentCenterName;

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> UpdateVendor(LineItem lineItem, CartProduct cartProduct)
        {
            lineItem.VendorId = cartProduct?.Product?.Vendor;

            return Task.FromResult(this);
        }

        public virtual Task<CartAggregate> UpdateOrganization(ShoppingCart cart, Member member)
        {
            if (member is Contact contact && cart.Type != XPurchaseConstants.ListTypeName)
            {
                cart.OrganizationId = contact.Organizations?.FirstOrDefault();
            }

            return Task.FromResult(this);
        }

        public virtual async Task<CartAggregate> UpdateCartDynamicProperties(IList<DynamicPropertyValue> dynamicProperties)
        {
            await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(Cart, dynamicProperties);

            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartItemDynamicProperties(string lineItemId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == lineItemId);
            if (lineItem != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(lineItem, dynamicProperties);
            }

            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartItemDynamicProperties(LineItem lineItem, IList<DynamicPropertyValue> dynamicProperties)
        {
            await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(lineItem, dynamicProperties);
            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartShipmentDynamicProperties(string shipmentId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var shipment = Cart.Shipments.FirstOrDefault(x => x.Id == shipmentId);
            if (shipment != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(shipment, dynamicProperties);
            }

            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartShipmentDynamicProperties(Shipment shipment, IList<DynamicPropertyValue> dynamicProperties)
        {
            await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(shipment, dynamicProperties);
            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartPaymentDynamicProperties(string paymentId, IList<DynamicPropertyValue> dynamicProperties)
        {
            var payment = Cart.Payments.FirstOrDefault(x => x.Id == paymentId);
            if (payment != null)
            {
                await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(payment, dynamicProperties);
            }

            return this;
        }

        public virtual async Task<CartAggregate> UpdateCartPaymentDynamicProperties(Payment payment, IList<DynamicPropertyValue> dynamicProperties)
        {
            await _dynamicPropertyUpdaterService.UpdateDynamicPropertyValues(payment, dynamicProperties);
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
                var tierPrice = product.Price.GetTierPrice(quantity);
                if (CheckPricePolicy(tierPrice))
                {
                    lineItem.SalePrice = tierPrice.ActualPrice.Amount;
                    lineItem.ListPrice = tierPrice.Price.Amount;
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

        /// <summary>
        /// Represents a price policy for a product. By default, product price should be greater than zero.
        /// </summary>
        protected virtual bool CheckPricePolicy(TierPrice tierPrice)
        {
            return tierPrice.Price.Amount > 0;
        }

        protected virtual async Task<CartAggregate> InnerAddLineItemAsync(LineItem lineItem, CartProduct product = null, IList<DynamicPropertyValue> dynamicProperties = null)
        {
            var existingLineItem = LineItems.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                await InnerChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity), product);

                existingLineItem.FulfillmentCenterId = lineItem.FulfillmentCenterId;
                existingLineItem.FulfillmentCenterName = lineItem.FulfillmentCenterName;

                lineItem = existingLineItem;
            }
            else
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }

            if (dynamicProperties != null)
            {
                await UpdateCartItemDynamicProperties(lineItem, dynamicProperties);
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

        protected virtual async Task<TaxProvider> GetActiveTaxProviderAsync()
        {
            if (Store?.Settings?.GetValue<bool>(StoreSetting.TaxCalculationEnabled) != true)
            {
                return null;
            }

            var storeTaxProviders = await _taxProviderSearchService.SearchAsync(new TaxProviderSearchCriteria
            {
                StoreIds = new[] { Cart.StoreId }
            });

            return storeTaxProviders?.Results.FirstOrDefault(x => x.IsActive);
        }

        /// <summary>
        /// Sets ListPrice and SalePrice for line item by Product price
        /// </summary>
        public virtual void SetLineItemTierPrice(ProductPrice productPrice, int quantity, LineItem lineItem)
        {
            if (productPrice == null)
            {
                return;
            }

            var tierPrice = productPrice.GetTierPrice(quantity);
            if (tierPrice.Price.Amount > 0)
            {
                lineItem.SalePrice = tierPrice.ActualPrice.Amount;
                lineItem.ListPrice = tierPrice.Price.Amount;
            }
        }

        #region ICloneable

        public virtual object Clone()
        {
            var result = (CartAggregate)MemberwiseClone();

            result.Cart = Cart?.CloneTyped();
            result.CartProducts = CartProducts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.CloneTyped());
            result.Currency = Currency.CloneTyped();
            result.Member = Member?.CloneTyped();
            result.Store = Store.CloneTyped();

            return result;
        }

        #endregion ICloneable
    }
}
