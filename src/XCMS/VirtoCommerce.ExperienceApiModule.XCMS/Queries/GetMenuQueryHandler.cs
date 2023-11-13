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
    public class GetMenuQueryHandler : IQueryHandler<GetMenuQuery, GetMenuResponse>
    {
        private readonly IMenuService _menuService;

        public GetMenuQueryHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<GetMenuResponse> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            var result = new GetMenuResponse();
            var menuLinkLists = (await _menuService.GetListsByStoreIdAsync(request.StoreId)).ToList();

            var menuLinkList = GetMenuLinkList(menuLinkLists, request.CultureName, request.Name);

            if (menuLinkList != null)
            {
                var expMenuLinkList = new Menu
                {
                    Name = menuLinkList.Name,
                    OuterId = menuLinkList.OuterId,
                };

                foreach (var menuLink in menuLinkList.MenuLinks)
                {
                    var childMenuLinkList = GetMenuLinkList(menuLinkLists, request.CultureName, menuLink.Title);
                    expMenuLinkList.Items.Add(GetMenuLink(menuLink, childMenuLinkList));
                }

                result.MenuList = expMenuLinkList;
            }

            return result;
        }

        private static MenuLinkList GetMenuLinkList(IEnumerable<MenuLinkList> lists, string cultureName, string name)
        {
            return lists.FirstOrDefault(x =>
                x.Language?.EqualsInvariant(cultureName) == true &&
                x.Name.EqualsInvariant(name));
        }

        private static MenuItem GetMenuLink(MenuLink link, MenuLinkList list)
        {
            return new MenuItem
            {
                Link = link,
                ChildItems = list?.MenuLinks?.Select(x => GetMenuLink(x, null)).ToList() ?? new List<MenuItem>()
            };
        }
    }
}
