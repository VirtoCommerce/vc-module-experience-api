using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.XCMS.Core.Models
{
    public class MenuItem
    {
        public virtual MenuLink Link { get; set; }
        public IList<MenuItem> ChildItems { get; set; }
    }
}
