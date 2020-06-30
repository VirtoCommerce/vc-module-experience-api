using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Core.Model;

namespace VirtoCommerce.XPurchase.Extensions
{
    public static class RewardExtensions
    {
        public static void ApplyRewards(this PaymentMethod paymentMethod, ICollection<PromotionReward> rewards)
            => paymentMethod.DiscountAmount = rewards
                .Where(r => r.RewardType == "PaymentReward" && r.IsValid)
                .OfType<PaymentReward>()
                .Where(r => r.PaymentMethod.IsNullOrEmpty() || r.PaymentMethod.EqualsInvariant(paymentMethod.Code))
                .Sum(reward => reward.GetRewardAmount(paymentMethod.Price - paymentMethod.DiscountAmount, 1));

        public static void ApplyRewards(this ShipmentMethod shipmentMethod, ICollection<PromotionReward> rewards)
            => shipmentMethod.DiscountAmount = rewards
                .Where(r => r.RewardType == "ShipmentReward" && r.IsValid)
                .OfType<ShipmentReward>()
                .Where(r => r.ShippingMethod.IsNullOrEmpty() || r.ShippingMethod.EqualsInvariant(shipmentMethod.ShipmentMethodCode))
                .Sum(reward => reward.GetRewardAmount(shipmentMethod.Price, 1));

        public static void ApplyRewards(this ShippingRate shippingRate, ICollection<PromotionReward> rewards)
            => shippingRate.DiscountAmount = rewards
                .Where(r => r.RewardType == "ShipmentReward" && r.IsValid)
                .OfType<ShipmentReward>()
                .Where(r => r.ShippingMethod.IsNullOrEmpty() || (shippingRate.ShippingMethod != null && r.ShippingMethod.EqualsInvariant(shippingRate.ShippingMethod.Code)))
                .Sum(reward => reward.GetRewardAmount(shippingRate.Rate, 1)); //TODO: Check if this correct value for Price
    }
}
