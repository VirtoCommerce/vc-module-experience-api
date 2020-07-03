using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XPurchase
{
    public class CartProduct : Entity
    {
        public CartProduct(CatalogProduct product)
        {
            Product = product;
            Id = product.Id;
        }

        public CatalogProduct Product { get; private set; }

        public ProductPrice Price { get; private set; }

        public IList<ProductPrice> AllPrices { get; private set; } = new List<ProductPrice>();

        /// <summary>
        /// Inventory for default fulfillment center
        /// </summary>
        public InventoryInfo Inventory { get; private set; }

        /// <summary>
        /// Inventory of all fulfillment centers.
        /// </summary>
        public IList<InventoryInfo> AllInventories { get; private set; } = new List<InventoryInfo>();

        public virtual long AvailableQuantity
        {
            get
            {
                long result = 0;

                if (Product.TrackInventory.GetValueOrDefault(true) && AllInventories != null)
                {
                    foreach (var inventory in AllInventories)
                    {
                        result += Math.Max(0, inventory.InStockQuantity - inventory.ReservedQuantity);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Apply prices to product
        /// </summary>
        /// <param name="prices"></param>
        /// <param name="currentCurrency"></param>
        /// <param name="allCurrencies"></param>
        public virtual void ApplyPrices(IEnumerable<Price> prices, Currency currency)
        {
            AllPrices.Clear();
            Price = null;

            AllPrices = prices.Where(x => x.ProductId == Id)
                              .Select(x =>
                              {
                                  var productPrice = new ProductPrice(currency)
                                  {
                                      PricelistId = x.PricelistId,
                                      ListPrice = new Money(x.List, currency),
                                      MinQuantity = x.MinQuantity
                                  };
                                  productPrice.SalePrice = x.Sale == null ? productPrice.ListPrice : new Money(x.Sale.GetValueOrDefault(), currency);

                                  return productPrice;
                              }).ToList();

            //group prices by currency
            var groupByCurrencyPrices = AllPrices.GroupBy(x => x.Currency).Where(x => x.Any());
            foreach (var currencyGroup in groupByCurrencyPrices)
            {
                //For each currency need get nominal price (with min qty)
                var orderedPrices = currencyGroup.OrderBy(x => x.MinQuantity ?? 0).ThenBy(x => x.ListPrice);
                var nominalPrice = orderedPrices.FirstOrDefault();
                //and add to nominal price other prices as tier prices
                nominalPrice.TierPrices.AddRange(orderedPrices.Select(x => new TierPrice(x.SalePrice, x.MinQuantity ?? 1)));
                //Add nominal price to product prices list
                AllPrices.Add(nominalPrice);
            }
            //Set current product price for current currency
            Price = AllPrices.FirstOrDefault(x => x.Currency.Equals(currency));
        }

        public virtual void ApplyInventories(IEnumerable<InventoryInfo> inventories, Store store)
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
