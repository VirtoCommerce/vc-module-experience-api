using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Binding;
using VirtoCommerce.XDigitalCatalog.Specifications;
using ProductPrice = VirtoCommerce.ExperienceApiModule.Core.Models.ProductPrice;

namespace VirtoCommerce.XDigitalCatalog
{
    public class ExpProduct
    {
        public string Id => IndexedProduct.Id;

        [BindIndexField(FieldName = "__object", BinderType = typeof(CatalogProductBinder))]
        public virtual CatalogProduct IndexedProduct { get; set; }

        [BindIndexField(FieldName = "__variations", BinderType = typeof(VariationsBinder))]
        public virtual IList<string> IndexedVariationIds { get; set; } = new List<string>();

        [BindIndexField(FieldName = "__prices", BinderType = typeof(PriceBinder))]
        public virtual IList<Price> IndexedPrices { get; set; } = new List<Price>();

        /// <summary>
        /// All parent categories ids concatenated with "/". E.g. (1/21/344)  relative for the given catalog
        /// </summary>
        public string Outline { get; set; }

        public string Slug { get; set; }


        public SeoInfo SeoInfo { get; set; }

        public bool IsBuyable
        {
            get
            {
                return new CatalogProductIsBuyableSpecification().IsSatisfiedBy(this);
            }
        }

        public bool IsAvailable
        {
            get
            {
                return new CatalogProductIsAvailableSpecification().IsSatisfiedBy(this);
            }
        }

        public bool IsInStock
        {
            get
            {
                return new CatalogProductIsInStockSpecification().IsSatisfiedBy(this);
            }
        }

        public IList<ProductPrice> AllPrices { get;  set; } = new List<ProductPrice>();

        /// <summary>
        /// Inventory of all fulfillment centers.
        /// </summary>
        public IList<InventoryInfo> AllInventories { get; set; } = new List<InventoryInfo>();

        /// <summary>
        /// Inventory for default fulfillment center
        /// </summary>
        public InventoryInfo Inventory { get; private set; }



        public virtual long AvailableQuantity
        {
            get
            {
                long result = 0;

                if (IndexedProduct.TrackInventory.GetValueOrDefault(true) && AllInventories != null)
                {
                    foreach (var inventory in AllInventories)
                    {
                        result += Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity);
                    }
                }
                return result;
            }
        }

        public virtual void ApplyRewards(CatalogItemAmountReward[] allRewards)
        {
            var productRewards = allRewards.Where(r => r.ProductId.IsNullOrEmpty() || r.ProductId.EqualsInvariant(Id));
            if (productRewards == null)
            {
                return;
            }

            var rewardsMap = AllPrices
                   .Select(x => x.Currency)
                   .Distinct()
                   .ToDictionary(x => x, x => productRewards);

            foreach (var productPrice in AllPrices)
            {
                var mappedRewards = rewardsMap[productPrice.Currency];
                productPrice.Discounts.Clear();
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

        public virtual void ApplyStoreInventories(IEnumerable<InventoryInfo> inventories, Store store)
        {
            if (inventories == null)
            {
                throw new ArgumentNullException(nameof(inventories));
            }
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            var availFullfilmentCentersIds = (store.AdditionalFulfillmentCenterIds ?? Array.Empty<string>()).Concat(new[] { store.MainFulfillmentCenterId });

            AllInventories.Clear();
            Inventory = null;
            AllInventories = inventories.Where(x => x.ProductId == Id && availFullfilmentCentersIds.Contains(x.FulfillmentCenterId)).ToList();

            Inventory = inventories.OrderByDescending(x => Math.Max(0, x.InStockQuantity - x.ReservedQuantity)).FirstOrDefault();

            if (store.MainFulfillmentCenterId != null)
            {
                Inventory = AllInventories.FirstOrDefault(x => x.FulfillmentCenterId == store.MainFulfillmentCenterId) ?? Inventory;
            }
        }
    }
}
