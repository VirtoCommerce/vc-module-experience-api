using System.Collections.Generic;
using AutoFixture;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.Tests.Helpers;
using VirtoCommerce.InventoryModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Tests.Helpers
{
    public class XDigitalCatalogMoqHelper : MoqHelper
    {
        protected ExpProduct GetExpProduct(ExpProductOptions options)
        {
            var product = new ExpProduct();

            product.IndexedProduct = new CatalogProduct()
            {
                IsActive = options.IsActive,
                IsBuyable = options.IsBuyable,
                TrackInventory = options.TrackInventory,
            };

            if (options.HasPrices)
            {
                var productPrice = _fixture.Create<ProductPrice>();
                productPrice.ListPrice = GetMoney(100);
                product.AllPrices = new List<ProductPrice>() { productPrice };
            }

            if (options.HasInventory)
            {
                var inventory = _fixture.Create<InventoryInfo>();
                inventory.AllowBackorder = options.AllowBackorder;
                inventory.AllowPreorder = options.AllowPreorder;
                inventory.InStockQuantity = options.InStockQuantity;
                inventory.ReservedQuantity = options.ReservedQuantity;
                product.AllInventories = new List<InventoryInfo>() { inventory };
            }

            return product;
        }

        public class ExpProductOptions
        {
            public bool IsActive { get; set; } = true;
            public bool IsBuyable { get; set; } = true;
            public bool TrackInventory { get; set; } = true;
            public bool HasPrices { get; set; } = true;
            public bool HasInventory { get; set; } = true;
            public bool AllowBackorder { get; set; } = true;
            public bool AllowPreorder { get; set; } = true;
            public long InStockQuantity { get; set; } = 100;
            public long ReservedQuantity { get; set; } = 50;
        }
    }
}
