using System;
using System.Collections.Generic;
using System.Linq;
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
            var result = await GetProductsByIdsAsync(cartAggr, ids, additionalResponseGroups);

            if (!result.IsNullOrEmpty())
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

                foreach (var cartProduct in result)
                {
                    //Apply inventories
                    cartProduct.ApplyInventories(loadInventoriesTask.Result.Results, cartAggr.Store);

                    //Apply prices
                    cartProduct.ApplyPrices(evalPricesTask.Result, cartAggr.Currency);
                }
            }
            return result;
        }

        public async Task<IEnumerable<CartProduct>> GetProductsByIdsAsync(CartAggregate cartAggr, string[] ids, string additionalResponseGroups = null)
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
            var products = await _productService.GetByIdsAsync(ids, (defaultResponseGroups | EnumUtility.SafeParseFlags(additionalResponseGroups, ItemResponseGroup.None)).ToString());

            return products
                .Select(x => new CartProduct(x))
                .ToList();
        }
    }
}
