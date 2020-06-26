using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.InventoryModule.Core.Model.Search;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.XPurchase.Services
{
    public class CartProductService : ICartProductService
    {
        private readonly IItemService _productService;
        private readonly IInventorySearchService _inventorySearchService;
        private readonly IPricingService _pricingService;
        private readonly IStoreService _storeService;

        private readonly IMapper _mapper;
        public CartProductService(
            IItemService productService
            , IInventorySearchService inventoryService
            , IPricingService pricingService
            , IStoreService storeService
            , IMapper mapper)
        {
            _productService = productService;
            _inventorySearchService = inventoryService;
            _pricingService = pricingService;
            _storeService = storeService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartProduct>> GetCartProductsByIdsAsync(ShoppingCart cart, string[] ids)
        {
            if(cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            if(ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var result = new List<CartProduct>();
            var products = await _productService.GetByIdsAsync(ids, (ItemResponseGroup.ItemAssets | ItemResponseGroup.ItemInfo | ItemResponseGroup.Outlines | ItemResponseGroup.Seo).ToString());
            if (!products.IsNullOrEmpty())
            {
                var store = await _storeService.GetByIdAsync(cart.StoreId);

                var loadInventoriesTask = _inventorySearchService.SearchInventoriesAsync(new InventorySearchCriteria
                {
                    ProductIds = ids,
                    Take = int.MaxValue
                });

                var pricesEvalContext = _mapper.Map<PriceEvaluationContext>(cart);
                pricesEvalContext.ProductIds = ids;
                pricesEvalContext.CatalogId = store.Catalog;

                var evalPricesTask = _pricingService.EvaluateProductPricesAsync(pricesEvalContext);

                await Task.WhenAll(loadInventoriesTask, evalPricesTask);
                var availFullfilmentCentersIds =  (store.AdditionalFulfillmentCenterIds ?? Array.Empty<string>()).Concat(new[] { store.MainFulfillmentCenterId });

                foreach (var product in products)
                {
                    //Apply inventories
                    var cartProduct = new CartProduct(product)
                    {
                        //TODO: Change these conditions to DDD specification
                        AllInventories = loadInventoriesTask.Result.Results.Where(x => x.ProductId == product.Id).Where(x => availFullfilmentCentersIds.Contains(x.FulfillmentCenterId)).ToList()
                    };
                    cartProduct.Inventory = cartProduct.AllInventories.OrderByDescending(x => Math.Max(0, x.InStockQuantity - x.ReservedQuantity)).FirstOrDefault();

                    if (store.MainFulfillmentCenterId != null)
                    {
                        cartProduct.Inventory = cartProduct.AllInventories.FirstOrDefault(x => x.FulfillmentCenterId == store.MainFulfillmentCenterId) ?? cartProduct.Inventory;
                    }

                    //Apply prices

                    var productPrices = evalPricesTask.Result.Where(x => x.ProductId == product.Id)
                                              .Select(x =>
                                              {
                                                  var currency = new Currency(new Language(cart.LanguageCode), x.Currency);
                                                  var productPrice = new ProductPrice(currency)
                                                  {
                                                      PricelistId = x.PricelistId,
                                                      ListPrice = new Money(x.List, currency),
                                                      MinQuantity = x.MinQuantity

                                                  };
                                                  productPrice.SalePrice = x.Sale == null ? productPrice.ListPrice : new Money(x.Sale.GetValueOrDefault(), currency);
                                                  return productPrice;
                                              });

                    cartProduct.ApplyPrices(productPrices, cart.Currency);
                    result.Add(cartProduct);
                }
            }
            return result;
        }
    }
}
