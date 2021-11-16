using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenusResponse
    {
        public IEnumerable<MenuLinkList> Menus { get; set; } = new List<MenuLinkList>();
    }
}
