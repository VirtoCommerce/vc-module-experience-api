using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Queries
{
    public class GetMenusResponse
    {
        public IEnumerable<Menu> Menus { get; set; } = new List<Menu>();
    }
}
