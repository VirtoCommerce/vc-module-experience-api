using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XCMS
{
    public class MenuItem
    {
        public virtual MenuLink Link { get; set; }
        public IList<MenuItem> ChildItems { get; set; }
    }
}
