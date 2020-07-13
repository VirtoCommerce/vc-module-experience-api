using AutoMapper;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Binding;

namespace VirtoCommerce.XDigitalCatalog.Mapping
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
