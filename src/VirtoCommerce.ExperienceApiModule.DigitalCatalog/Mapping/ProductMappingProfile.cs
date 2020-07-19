using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Binding;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class ProductMappingProfile : Profile
    {
        private static readonly ExpProductBinder _productBinder = AbstractTypeFactory<ExpProductBinder>.TryCreateInstance();
        private static readonly ExpCategoryBinder _categoryBinder = AbstractTypeFactory<ExpCategoryBinder>.TryCreateInstance();

        public ProductMappingProfile()
        {
            CreateMap<SearchDocument, ExpProduct>().ConvertUsing((doc, facet, context) => _productBinder.BindModel(doc) as ExpProduct);

            CreateMap<SearchDocument, ExpCategory>().ConvertUsing((doc, facet, context) => _categoryBinder.BindModel(doc) as ExpCategory);
        }
    }
}
