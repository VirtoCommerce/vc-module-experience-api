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
        
        protected virtual ItemResponseGroup DefaultResponseGroups { get; set; } = ItemResponseGroup.ItemAssets | ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines | ItemResponseGroup.Seo;
        protected virtual int DefaultPageSize { get; set; } = 50;
        
        public CartProductService(IItemService productService, IInventorySearchService inventoryService, IPricingService pricingService, IMapper mapper)
        {
            _productService = productService;
            _inventorySearchService = inventoryService;
            _pricingService = pricingService;
            _mapper = mapper;
        }

        public async Task<IList<CartProduct>> GetCartProductsByIdsAsync(CartAggregate aggregate, IEnumerable<string> ids)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var products = await GetProductsByIdsAsync(ids);
            var cartProducts = await GetCartProductsAsync(products);
            
            var evalPricesTask = AddPricesToCartProductAsync(aggregate, cartProducts);
            var loadInventoriesTask = AddInventorysToCartProductAsync(aggregate, cartProducts);
            await Task.WhenAll(loadInventoriesTask, evalPricesTask);
            
            return cartProducts;
        }

        protected virtual async Task<IList<CatalogProduct>> GetProductsByIdsAsync(IEnumerable<string> ids)
        {
            return await _productService.GetByIdsAsync(ids.ToArray(), DefaultResponseGroups.ToString());
        }

        protected virtual Task<List<CartProduct>> GetCartProductsAsync(IEnumerable<CatalogProduct> catalogProducts)
        {
            return Task.FromResult(catalogProducts.Select(x => new CartProduct(x)).ToList());
        }

        protected virtual async Task AddPricesToCartProductAsync(CartAggregate cartAggregate, List<CartProduct> products)
        {
            if (products.IsNullOrEmpty())
            {
                return;
            }
            
            var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cartAggregate);
            pricesEvalContext.ProductIds = products.Select(x => x.Id).ToArray();
            
            var evalPricesTask = await _pricingService.EvaluateProductPricesAsync(pricesEvalContext);

            foreach (var cartProduct in products)
            {
                cartProduct.ApplyPrices(evalPricesTask, cartAggregate.Currency);
            }
        }

        protected virtual async Task AddInventorysToCartProductAsync(CartAggregate cartAggregate, List<CartProduct> products)
        {
            if (products.IsNullOrEmpty())
            {
                return;
            }

            var ids = products.Select(x => x.Id).ToArray();
            
            var countResult = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
            {
                ProductIds = ids,
                Skip = 0,
                Take = DefaultPageSize
            });
            
            var allLoadInventories = countResult.Results.ToList();

            if (countResult.TotalCount > DefaultPageSize)
            {
                for (var i = DefaultPageSize; i < countResult.TotalCount; i += DefaultPageSize)
                {
                    var loadInventoriesTask = await _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                    {
                        ProductIds = ids,
                        Skip = i,
                        Take = DefaultPageSize
                    });
                    
                    allLoadInventories.AddRange(loadInventoriesTask.Results);
                }
            }

            foreach (var cartProduct in products)
            {
                cartProduct.ApplyInventories(allLoadInventories, cartAggregate.Store);
            }
        }
    }
}
