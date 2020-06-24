using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.XPurchase.Domain.Converters;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Enums;
using VirtoCommerce.XPurchase.Models.Extensions;

namespace VirtoCommerce.XPurchase.Domain.Services
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly IItemService _itemsService;

        public ProductsRepository(IItemService itemsService)
        {
            _itemsService = itemsService;
        }

        public virtual async Task<IEnumerable<Product>> GetProductsAsync(string[] ids, Currency currency, Language language, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            if (ids.IsNullOrEmpty())
            {
                return Enumerable.Empty<Product>();
            }

            var products = await _itemsService
                .GetByIdsAsync(ids, ((int)responseGroup).ToString())
                .ConfigureAwait(false);

            return products.Select(x => x.ToProduct(language, currency));
        }
    }
}
