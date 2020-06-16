using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Catalog;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Common;
using VirtoCommerce.ExperienceApiModule.XPurchase.Models.Enums;

namespace VirtoCommerce.ExperienceApiModule.XPurchase.Models.Cart.Services
{
    /// <summary>
    /// Represent abstraction to search in catalog api (products, categories etc)
    /// </summary>
    public interface ICatalogService
    {
        Task<IEnumerable<Product>> GetProductsAsync(string[] ids, Currency currency, Language language, ItemResponseGroup responseGroup = ItemResponseGroup.None);
    }
}


