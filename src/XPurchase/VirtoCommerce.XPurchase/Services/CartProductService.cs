using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;

namespace VirtoCommerce.XPurchase.Services
{
    public class CartProductService : ICartProductService
    {
        private readonly IItemService _productService;
        private readonly IInventorySearchService _inventorySearchService;
        private readonly IPricingService _pricingService;


        private readonly IMapper _mapper;
        public CartProductService(
            IItemService productService
            , IInventorySearchService inventoryService
            , IPricingService pricingService
            , IMapper mapper)
        {
            _productService = productService;
            _inventorySearchService = inventoryService;
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(CartAggregate cartAggr, string[] ids, string additionalResponseGroups = null)
        {
            if (cartAggr == null)
            {
                throw new ArgumentNullException(nameof(cartAggr));
            }
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var defaultResponseGroups = ItemResponseGroup.ItemAssets | ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines | ItemResponseGroup.Seo;

            var result = new List<CartProduct>();
            var products = await _productService.GetByIdsAsync(ids, (defaultResponseGroups | EnumUtility.SafeParseFlags(additionalResponseGroups, ItemResponseGroup.None)).ToString());
            if (!products.IsNullOrEmpty())
            {
                var loadInventoriesTask = _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                {
                    ProductIds = ids,
                    //Do not use int.MaxValue use only 10 items per requested product
                    //TODO: Replace to pagination load
                    Take = Math.Min(ids.Length * 10, 500)
                });

                var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cartAggr);
                pricesEvalContext.ProductIds = ids;             
                var evalPricesTask = _pricingService.EvaluateProductPricesAsync(pricesEvalContext);

                await Task.WhenAll(loadInventoriesTask, evalPricesTask);
              
                foreach (var product in products)
                {
                    var cartProduct = new CartProduct(product);
                    //Apply inventories
                    cartProduct.ApplyInventories(loadInventoriesTask.Result.Results, cartAggr.Store);

                    //Apply prices
                    cartProduct.ApplyPrices(evalPricesTask.Result, cartAggr.Currency);
                    result.Add(cartProduct);
                }
            }
            return result;
        }
    }
}
