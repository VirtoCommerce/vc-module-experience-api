using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class RewardExtensions
    {
        public static void ApplyRewards(this List<ProductPrice> productPrices, CatalogItemAmountReward[] rewards)
        {
            if (rewards.IsNullOrEmpty())
            {
                return;
            }

            var rewardsMap = productPrices
                .Select(x => x.Currency)
                .Distinct()
                .ToDictionary(x => x, x => rewards);

            foreach (var productPrice in productPrices)
            {
                var mappedRewards = rewardsMap[productPrice.Currency];

                productPrice.DiscountAmount = new Money(Math.Max(0, (productPrice.ListPrice - productPrice.SalePrice).Amount), productPrice.Currency);

                foreach (var reward in mappedRewards)
                {
                    foreach (var tierPrice in productPrice.TierPrices)
                    {
                        tierPrice.DiscountAmount = new Money(Math.Max(0, (productPrice.ListPrice - tierPrice.Price).Amount), productPrice.Currency);
                    }

                    if (!reward.IsValid)
                    {
                        continue;
                    }

                    var priceAmount = (productPrice.ListPrice - productPrice.DiscountAmount).Amount;

                    var discount = new Discount
                    {
                        DiscountAmount = reward.GetRewardAmount(priceAmount, 1),
                        Description = reward.Promotion.Description,
                        Coupon = reward.Coupon,
                        PromotionId = reward.Promotion.Id
                    };

                    productPrice.Discounts.Add(discount);

                    if (discount.DiscountAmount > 0)
                    {
                        productPrice.DiscountAmount += discount.DiscountAmount;

                        foreach (var tierPrice in productPrice.TierPrices)
                        {
                            tierPrice.DiscountAmount += reward.GetRewardAmount(tierPrice.Price.Amount, 1);
                        }
                    }
                }
            }
        }
    }
}
