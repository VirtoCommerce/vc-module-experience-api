using System.Collections.Generic;

namespace VirtoCommerce.XCMS.Core.Models
{
    public class GetMenusResponse
    {
        public IEnumerable<Menu> Menus { get; set; } = new List<Menu>();
    }
}
