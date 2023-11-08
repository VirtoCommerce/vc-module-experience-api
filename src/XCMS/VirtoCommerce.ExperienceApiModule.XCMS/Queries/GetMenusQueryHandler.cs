using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenusQueryHandler : IQueryHandler<GetMenusQuery, GetMenusResponse>
    {
        private readonly IMenuLinkListSearchService _menuService;

        public GetMenusQueryHandler(IMenuLinkListSearchService menuService)
        {
            _menuService = menuService;
        }

        public async Task<GetMenusResponse> Handle(GetMenusQuery request, CancellationToken cancellationToken)
        {
            var criteria = AbstractTypeFactory<MenuLinkListSearchCriteria>.TryCreateInstance();
            criteria.StoreId = request.StoreId;

            var result = await _menuService.SearchAllAsync(criteria);

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
                Menus = result.Select(x => new Menu
                {
                    Name = x.Name,
                    OuterId = x.OuterId,
                    Items = x.MenuLinks?.Select(y => new MenuItem
                    {
                        Link = y
                    }).ToList() ?? new List<MenuItem>()
                }),
            };
        }
    }
}
