using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Core.Models
{
    public class LoadCategoryResponse
    {
        public LoadCategoryResponse(ICollection<ExpCategory> expCategories)
        {
            Categories = expCategories;
        }

        public ICollection<ExpCategory> Categories { get; private set; }
    }
}
