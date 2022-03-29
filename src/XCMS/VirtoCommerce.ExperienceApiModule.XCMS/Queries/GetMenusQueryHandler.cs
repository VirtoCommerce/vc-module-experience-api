using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenusQueryHandler : IQueryHandler<GetMenusQuery, GetMenusResponse>
    {
        private readonly IMenuService _menuService;

        public GetMenusQueryHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<GetMenusResponse> Handle(GetMenusQuery request, CancellationToken cancellationToken)
        {
            var result = await _menuService.GetListsByStoreIdAsync(request.StoreId);

            if (!string.IsNullOrEmpty(request.CultureName))
            {
                result = result.Where(x => x.Language?.EqualsInvariant(request.CultureName) == true).ToList();
            }

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                result = result.Where(x => x.Name.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return new GetMenusResponse
            {
                Menus = result,
            };
        }
    }
}
