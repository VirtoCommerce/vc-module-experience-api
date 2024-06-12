using System.Collections.Generic;

namespace VirtoCommerce.XCMS.Core.Models
{
    public class Menu
    {
        public string Name { get; set; }
        public string OuterId { get; set; }
        public IList<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
