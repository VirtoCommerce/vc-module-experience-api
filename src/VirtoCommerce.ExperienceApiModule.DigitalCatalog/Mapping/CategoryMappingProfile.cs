using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<SearchDocument, ExpCategory>().ConvertUsing(src => new GenericModelBinder<ExpCategory>().BindModel(src) as ExpCategory);
        }
    }
}
