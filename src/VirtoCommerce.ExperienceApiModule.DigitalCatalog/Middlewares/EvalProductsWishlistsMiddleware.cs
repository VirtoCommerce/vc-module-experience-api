using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalProductsWishlistsMiddleware : IAsyncMiddleware<SearchProductResponse>
    {
        private readonly IWishlistService _wishlistService;
        private readonly IMemberResolver _memberResolver;

        public EvalProductsWishlistsMiddleware(IWishlistService wishlistService, IMemberResolver memberResolver)
        {
            _wishlistService = wishlistService;
            _memberResolver = memberResolver;
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
                var contact = await _memberResolver.ResolveMemberByIdAsync(query.UserId) as Contact;
                var organizationId = contact?.Organizations?.FirstOrDefault();

                var wishlistsByProducts = await _wishlistService.FindWishlistsByProductsAsync(query.UserId, organizationId, query.StoreId, productIds);

                if (wishlistsByProducts.Any())
                {
                    parameter.Results.Apply((item) =>
                    {
                        if (wishlistsByProducts.TryGetValue(item.Id, out var wishlistIds))
                        {
                            item.WishlistIds = wishlistIds;
                        }
                        item.InWishlist = item.WishlistIds.Any();
                    });
                }
            }

            await next(parameter);
        }
    }
}
