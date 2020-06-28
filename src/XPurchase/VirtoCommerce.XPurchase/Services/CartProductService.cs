using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Currency;
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

            var result = new List<CartProduct>();
            var products = await _productService.GetByIdsAsync(ids, (ItemResponseGroup.ItemAssets | ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines | ItemResponseGroup.Seo).ToString());
            if (!products.IsNullOrEmpty())
            {
                var loadInventoriesTask = _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                {
                    ProductIds = ids,
                    Take = int.MaxValue
                });

                var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cartAggr);
                pricesEvalContext.ProductIds = ids;             
                var evalPricesTask = _pricingService.EvaluateProductPricesAsync(pricesEvalContext);

                await Task.WhenAll(loadInventoriesTask, evalPricesTask);
                var availFullfilmentCentersIds = (cartAggr.Store.AdditionalFulfillmentCenterIds ?? Array.Empty<string>()).Concat(new[] { cartAggr.Store.MainFulfillmentCenterId });

                foreach (var product in products)
                {
                    //Apply inventories
                    var cartProduct = new CartProduct(product)
                    {
                        //TODO: Change these conditions to DDD specification
                        AllInventories = loadInventoriesTask.Result.Results.Where(x => x.ProductId == product.Id).Where(x => availFullfilmentCentersIds.Contains(x.FulfillmentCenterId)).ToList()
                    };
                    cartProduct.Inventory = cartProduct.AllInventories.OrderByDescending(x => Math.Max(0, x.InStockQuantity - x.ReservedQuantity)).FirstOrDefault();

                    if (cartAggr.Store.MainFulfillmentCenterId != null)
                    {
                        cartProduct.Inventory = cartProduct.AllInventories.FirstOrDefault(x => x.FulfillmentCenterId == cartAggr.Store.MainFulfillmentCenterId) ?? cartProduct.Inventory;
                    }

                    //Apply prices
                    var productPrices = evalPricesTask.Result.Where(x => x.ProductId == product.Id)
                                              .Select(x =>
                                              {
                                                  var productPrice = new ProductPrice(cartAggr.Currency)
                                                  {
                                                      PricelistId = x.PricelistId,
                                                      ListPrice = new Money(x.List, cartAggr.Currency),
                                                      MinQuantity = x.MinQuantity

                                                  };
                                                  productPrice.SalePrice = x.Sale == null ? productPrice.ListPrice : new Money(x.Sale.GetValueOrDefault(), cartAggr.Currency);
                                                  return productPrice;
                                              });

                    cartProduct.ApplyPrices(productPrices, cartAggr.Currency);
                    result.Add(cartProduct);
                }
            }
            return result;
        }
    }
}
