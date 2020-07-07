using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Queries
{
    public class LoadProductResponse
    {
        public ICollection<ExpProduct> Products { get; set; } = new List<ExpProduct>();
    }
}
