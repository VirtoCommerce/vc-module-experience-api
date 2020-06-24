using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Enums;

namespace VirtoCommerce.XPurchase.Models.Cart.Services
{
    /// <summary>
    /// Represent abstraction to search in catalog api (products, categories etc)
    /// </summary>
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync(string[] ids, Currency currency, Language language, ItemResponseGroup responseGroup = ItemResponseGroup.None);
    }
}


