using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.InventoryModule.Core.Model;
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
        protected virtual ItemResponseGroup DefaultResponseGroups { get; set; } = ItemResponseGroup.ItemAssets | ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines | ItemResponseGroup.Seo;
        protected virtual int DefaultPageSize { get; set; } = 50;

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

        public async Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(CartAggregate cartAggr, string[] ids)
        {
            if (cartAggr == null)
            {
                throw new ArgumentNullException(nameof(cartAggr));
            }
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var products = await GetProductsByIdsAsync(ids);
            var cartProductWithPrices = await AddPricesToCartProductAsync(cartAggr, products.ToList());
            var cartProductWithInventorys = await AddInventorysToCartProductAsync(cartAggr, products.ToList());
            return cartProductWithPrices.AddRange(cartProductWithInventorys);
        }

        protected virtual async Task<IEnumerable<CatalogProduct>> GetProductsByIdsAsync(string[] ids)
        {
            return await _productService.GetByIdsAsync(ids, DefaultResponseGroups.ToString());
        }

        protected virtual async Task<IList<CartProduct>> AddPricesToCartProductAsync(CartAggregate cartAggregate, List<CatalogProduct> products)
        {
            var result = new List<CartProduct>();
            if (!products.IsNullOrEmpty())
            {
                var ids = products.Select(x => x.Id).ToArray();
                var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cartAggregate);
                pricesEvalContext.ProductIds = ids;
                var evalPricesTask = await _pricingService.EvaluateProductPricesAsync(pricesEvalContext);

                foreach (var product in products)
                {
                    var cartProduct = new CartProduct(product);
                    //Apply prices
                    cartProduct.ApplyPrices(evalPricesTask, cartAggregate.Currency);
                    result.Add(cartProduct);
                }
            }
            return result;
        }

        protected virtual async Task<IList<CartProduct>> AddInventorysToCartProductAsync(CartAggregate cartAggregate, List<CatalogProduct> products)
        {
            var pageNumber = 1;
            var skip = 0;
            var take = DefaultPageSize;

            var result = new List<CartProduct>();
            var allLoadInventories = new List<InventoryInfo>();

            if (!products.IsNullOrEmpty())
            {
                var ids = products.Select(x => x.Id).ToArray();
                var totalCounts = (await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria { ProductIds = ids })).Results.Count;

                while (skip < totalCounts)
                {
                    var loadInventoriesTask = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                    {
                        ProductIds = ids,
                        Skip = skip,
                        Take = take
                    });
                    allLoadInventories.AddRange(loadInventoriesTask.Results);
                    pageNumber++;
                    skip = (pageNumber - 1) * DefaultPageSize;
                    take = Math.Min(DefaultPageSize, totalCounts - skip);
                }

                foreach (var product in products)
                {
                    var cartProduct = new CartProduct(product);
                    //Apply inventories
                    cartProduct.ApplyInventories(allLoadInventories, cartAggregate.Store);
                    result.Add(cartProduct);
                }
            }
            return result;
        }
    }
}
