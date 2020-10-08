using System.Collections.Generic;

namespace VirtoCommerce.XDigitalCatalog.Queries
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
