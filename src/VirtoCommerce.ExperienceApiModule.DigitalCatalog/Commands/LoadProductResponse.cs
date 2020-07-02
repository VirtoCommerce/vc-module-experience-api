using System.Collections.Generic;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Index;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Requests
{
    public class LoadProductResponse
    {
        public ICollection<ExpProduct> Products { get; set; } = new List<ExpProduct>();
    }
}
