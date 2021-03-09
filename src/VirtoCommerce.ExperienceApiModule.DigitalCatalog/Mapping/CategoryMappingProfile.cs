using AutoMapper;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<SearchDocument, ExpCategory>().ConvertUsing((src, dest, context) =>
            {
                var expCategory = new GenericModelBinder<ExpCategory>().BindModel(src) as ExpCategory;

                if (expCategory != null)
                {
                    expCategory.Store = context.Options.Items["store"] as Store;
                    expCategory.CultureName = context.Options.Items["cultureName"].ToString();
                }

                return expCategory;
            });
        }
    }
}
