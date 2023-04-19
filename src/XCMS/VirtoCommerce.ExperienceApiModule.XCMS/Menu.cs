using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.XCMS
{
    public class Menu
    {
        public string Name { get; set; }
        public string OuterId { get; set; }
        public IList<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
