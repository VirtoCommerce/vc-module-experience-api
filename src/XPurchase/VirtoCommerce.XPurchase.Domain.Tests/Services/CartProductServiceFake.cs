using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Services;
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
            ILoadUserToEvalContextService loadUserToEvalContextService,
            IMediator mediator)
            : base(productService, inventoryService, pricingEvaluatorService, mapper, loadUserToEvalContextService, mediator)
        {
        }

        internal Task<IList<CatalogProduct>> GetProductsByIdsFakeAsync(IList<string> ids)
        {
            return base.GetProductsByIdsAsync(ids);
        }
    }
}
