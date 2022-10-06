using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Pipelines;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.PricingModule.Core.Services;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.XPurchase.Tests.Services
{
    public class CartProductServiceFake : CartProductService
    {
        public CartProductServiceFake(IItemService productService,
            IInventorySearchService inventoryService,
            IPricingEvaluatorService pricingEvaluatorService,
            IMapper mapper,
            IGenericPipelineLauncher pipeline)
            : base(productService, inventoryService, pricingEvaluatorService, mapper, pipeline)
        {
        }

        internal Task<IList<CatalogProduct>> GetProductsByIdsFakeAsync(IEnumerable<string> ids)
        {
            return base.GetProductsByIdsAsync(ids);
        }
    }
}
