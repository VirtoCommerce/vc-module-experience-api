using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.XPurchase.Models.Cart.Services;
using VirtoCommerce.XPurchase.Models.Catalog;
using VirtoCommerce.XPurchase.Models.Common;
using VirtoCommerce.XPurchase.Models.Enums;

namespace VirtoCommerce.XPurchase.Domain.Services
{
    public class CatalogService : ICatalogService
    {
        public virtual async Task<IEnumerable<Product>> GetProductsAsync(string[] ids, Currency currency, Language language, ItemResponseGroup responseGroup = ItemResponseGroup.None)
        {
            // todo: Need implementation
            return new List<Product>()
            {
                new Product(currency, language)
                {
                    Id = ids?[0] ?? "777",
                    Name = "Hi! I Mocked!",
                },
                new Product(currency, language)
                {
                    Id = ids?[1] ?? "888",
                    Name = "Hi! I Mocked!",
                }
            };
        }
    }
}
