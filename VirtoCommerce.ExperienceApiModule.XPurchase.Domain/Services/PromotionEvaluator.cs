using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Converters;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Extensions;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Marketing.Services;
using VirtoCommerce.MarketingModule.Core.Services;


namespace VirtoCommerce.ExperienceApiModule.XPurchase.Domain.Services
{
    public class PromotionEvaluator : IPromotionEvaluator
    {
        private readonly IMarketingPromoEvaluator _marketingPromoEvaluator;

        public PromotionEvaluator(IMarketingPromoEvaluator marketingPromoEvaluator)
        {
            _marketingPromoEvaluator = marketingPromoEvaluator;
        }

        #region IPromotionEvaluator Members

        public virtual async Task EvaluateDiscountsAsync(PromotionEvaluationContext promotionEvaluationContext,
            IEnumerable<IDiscountable> owners)
        {
            var context = Map(promotionEvaluationContext);

            var rewards = await _marketingPromoEvaluator
                .EvaluatePromotionAsync(context)
                .ConfigureAwait(false);

            ApplyRewards(rewards.Rewards, owners);
        }

        #endregion IPromotionEvaluator Members

        protected virtual void ApplyRewards(IEnumerable<MarketingModule.Core.Model.Promotions.PromotionReward> rewards, IEnumerable<IDiscountable> owners)
        {
            if (rewards == null)
            {
                return;
            }

            var rewardsMap = owners
                .Select(x => x.Currency)
                .Distinct()
                .ToDictionary(
                    x => x,
                    currency => rewards
                        .Select(reward => ToPromotionReward(reward, currency))
                        .ToArray());

            foreach (var owner in owners)
            {
                owner.ApplyRewards(rewardsMap[owner.Currency]);
            }
        }

        private static MarketingModule.Core.Model.Promotions.PromotionEvaluationContext Map(PromotionEvaluationContext promotionEvaluationContext)
        {
            var result = new MarketingModule.Core.Model.Promotions.PromotionEvaluationContext
            {
                StoreId = promotionEvaluationContext.StoreId,
                Language = promotionEvaluationContext.Language?.CultureName,
                Currency = promotionEvaluationContext.Currency?.Code,
                IsFirstTimeBuyer = promotionEvaluationContext.User.IsFirstTimeBuyer,
                IsRegisteredUser = promotionEvaluationContext.User.IsRegisteredUser,
                IsEveryone = true,
                CustomerId = promotionEvaluationContext.User.Id,
                UserGroups = promotionEvaluationContext?.User?.Contact?.UserGroups?.ToArray()
            };

            if (promotionEvaluationContext.Cart != null)
            {
                result.CartPromoEntries = promotionEvaluationContext.Cart.Items.Select(x => x.ToProductPromoEntryDto()).ToList();

                result.CartTotal = /*(double)*/promotionEvaluationContext.Cart.SubTotal.Amount; // Maybe it was rounding?
                result.Coupons = promotionEvaluationContext.Cart.Coupons?.Select(c => c.Code).ToList();
                //Set cart line items as default promo items
                result.PromoEntries = result.CartPromoEntries;

                if (!promotionEvaluationContext.Cart.Shipments.IsNullOrEmpty())
                {
                    var shipment = promotionEvaluationContext.Cart.Shipments.First();
                    result.ShipmentMethodCode = shipment.ShipmentMethodCode;
                    result.ShipmentMethodOption = shipment.ShipmentMethodOption;
                    result.ShipmentMethodPrice = /*(double)*/shipment.Price.Amount; // Maybe it was rounding?
                }
                if (!promotionEvaluationContext.Cart.Payments.IsNullOrEmpty())
                {
                    var payment = promotionEvaluationContext.Cart.Payments.First();
                    result.PaymentMethodCode = payment.PaymentGatewayCode;
                    result.PaymentMethodPrice = /*(double)*/payment.Price.Amount; // Maybe it was rounding?
                }
            }

            if (!promotionEvaluationContext.Products.IsNullOrEmpty())
            {
                result.PromoEntries = promotionEvaluationContext.Products.Select(x => ToProductPromoEntryDto(x)).ToList();
            }

            if (promotionEvaluationContext.Product != null)
            {
                result.PromoEntry = ToProductPromoEntryDto(promotionEvaluationContext.Product);
            }

            return result;
        }

        // todo: move to catalog extensions
        private static MarketingModule.Core.Model.Promotions.ProductPromoEntry ToProductPromoEntryDto(Product product)
            => new MarketingModule.Core.Model.Promotions.ProductPromoEntry
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Outline = product.Outline,
                Code = product.Sku,
                ProductId = product.Id,
                Quantity = 1,
                InStockQuantity = product.Inventory != null && product.Inventory.InStockQuantity.HasValue
                        ? (int)product.Inventory.InStockQuantity.Value
                        : 0,
                Variations = product.Variations?.Select(ToProductPromoEntryDto).ToList(),
                Discount = product.Price != null ? /*(double)*/product.Price.DiscountAmount.Amount : 0, // Maybe it was rounding?
                Price = product.Price != null ? /*(double)*/product.Price.SalePrice.Amount : 0, // Maybe it was rounding?
            };


        // todo: move to marketing extensions
        public static PromotionReward ToPromotionReward(MarketingModule.Core.Model.Promotions.PromotionReward rewardDto, Currency currency)
        {
            //todo: check with storefront implementation and v3 of marketing module
            var result = new PromotionReward
            {
                //CategoryId = rewardDto.CategoryId,
                Coupon = rewardDto.Coupon,
                Description = rewardDto.Description,
                //IsValid = rewardDto.IsValid ?? false,
                //LineItemId = rewardDto.LineItemId,
                //MeasureUnit = rewardDto.MeasureUnit,
                //ProductId = rewardDto.ProductId,
                PromotionId = rewardDto.PromotionId,
                //Quantity = rewardDto.Quantity ?? 0,
                //MaxLimit = (decimal)(rewardDto.MaxLimit ?? 0),
                //Amount = (decimal)(rewardDto.Amount ?? 0),
                //AmountType = EnumUtility.SafeParse(rewardDto.AmountType, AmountType.Absolute),
                CouponAmount = new Money(rewardDto.CouponAmount, currency),
                CouponMinOrderAmount = new Money(rewardDto.CouponMinOrderAmount ?? 0, currency),
                Promotion = ToPromotion(rewardDto.Promotion),// todo: move to extensions
                RewardType = EnumUtility.SafeParse(rewardDto.RewardType, PromotionRewardType.CatalogItemAmountReward),
                //ShippingMethodCode = rewardDto.ShippingMethod,
                //ConditionalProductId = rewardDto.ConditionalProductId,
                //ForNthQuantity = rewardDto.ForNthQuantity,
                //InEveryNthQuantity = rewardDto.InEveryNthQuantity,
            };

            return result;
        }

        // todo: move to extensions
        public static Promotion ToPromotion(MarketingModule.Core.Model.Promotions.Promotion promotionDto)
        {
            var result = new Promotion
            {
                Id = promotionDto.Id,
                Name = promotionDto.Name,
                Description = promotionDto.Description,
            };

            return result;
        }
    }
}
