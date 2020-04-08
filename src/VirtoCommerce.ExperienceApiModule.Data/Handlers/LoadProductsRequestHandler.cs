using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core;
using VirtoCommerce.ExperienceApiModule.Core.Requests;

namespace VirtoCommerce.ExperienceApiModule.Data.Handlers
{
    public class LoadProductsRequestHandler : IRequestHandler<LoadProductRequest, LoadProductResponse>
    {
        private readonly IItemService _itemService;
        public LoadProductsRequestHandler(IItemService itemService)
        {
            _itemService = itemService;
        }

        public virtual async Task<LoadProductResponse> Handle(LoadProductRequest request, CancellationToken cancellationToken)
        {
            var products = await _itemService.GetByIdsAsync(request.Ids, request.ResponseGroup);
            foreach(var product in products)
            {
                product.OuterId = "Virto";
            }
            return new LoadProductResponse { Products  = products };
        }
    }
}
