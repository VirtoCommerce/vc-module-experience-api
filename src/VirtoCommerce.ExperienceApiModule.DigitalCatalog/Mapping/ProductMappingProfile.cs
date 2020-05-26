using AutoMapper;
using VirtoCommerce.ExperienceApiModule.DigitalCatalog.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.DigitalCatalog.Mapping
{
    public class ProductMappingProfile : Profile
    {
        private static ExpProductBinder _productBinder = AbstractTypeFactory<ExpProductBinder>.TryCreateInstance();
        public ProductMappingProfile()
        {
            CreateMap<SearchDocument, ExpProduct>().ConvertUsing((doc, facet, context) =>
            {
                return _productBinder.BindModel(doc) as ExpProduct;
            });

        }
    }
}
