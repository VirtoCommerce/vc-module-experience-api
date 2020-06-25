using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.XPurchase
{
    public class CartProduct : Entity
    {
        public CartProduct(CatalogProduct product)
        {
            Product = product;
        }
        public CatalogProduct Product { get; private set; }

        public ProductPrice Price { get; set; }

        public IList<ProductPrice> AllPrices { get; set; } = new List<ProductPrice>();

        /// <summary>
        /// Inventory for default fulfillment center
        /// </summary>
        public InventoryInfo Inventory { get; set; }

        /// <summary>
        /// Inventory of all fulfillment centers.
        /// </summary>
        public IList<InventoryInfo> AllInventories { get; set; }

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
        public void ApplyPrices(IEnumerable<ProductPrice> prices, string currency)
        {
            AllPrices.Clear();
            Price = null;

            //group prices by currency
            var groupByCurrencyPrices = prices.GroupBy(x => x.Currency).Where(x => x.Any());
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
            Price = AllPrices.FirstOrDefault(x => x.Currency.Code == currency);
        }             

    }
}
