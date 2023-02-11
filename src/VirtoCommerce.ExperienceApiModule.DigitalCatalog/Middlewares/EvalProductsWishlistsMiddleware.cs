using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsWishlistsMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IWishlistService _wishlistService;

        public EvalProductsWishlistsMiddleware(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        public async Task Run(SearchProductResponse parameter, Func<SearchProductResponse, Task> next)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var query = parameter.Query;
            if (query == null)
            {
                throw new OperationCanceledException("Query must be set");
            }

            var productIds = parameter.Results.Select(x => x.Id).ToArray();
            var responseGroup = EnumUtility.SafeParse(query.GetResponseGroup(), ExpProductResponseGroup.None);
            // If products availabilities requested
            if (responseGroup.HasFlag(ExpProductResponseGroup.LoadWishlists) &&
                productIds.Any())
            {
                var productIdsInWishLists = await _wishlistService.FindProductsInWishlistsAsync(query.UserId, query.StoreId, productIds);

                if (productIdsInWishLists.Any())
                {
                    parameter.Results.Apply((item) =>
                    {
                        item.InWishlist = productIdsInWishLists.Contains(item.Id);
                    });
                }
            }

            await next(parameter);
        }
    }
}
