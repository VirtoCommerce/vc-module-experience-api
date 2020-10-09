using System;
using System.Linq;
using AutoMapper;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.ExperienceApiModule.Core.Binding;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.XDigitalCatalog.Extensions;

namespace VirtoCommerce.XDigitalCatalog.Mapping
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {        
            CreateMap<SearchDocument, ExpCategory>().ConvertUsing((src, dest, context) =>
            {
                if (!context.Items.TryGetValue("store", out var storeObj))
                {
                    throw new OperationCanceledException("store must be set");
                }
                var store = storeObj as Store;
                context.Items.TryGetValue("language", out var language);
                language = language ?? store.DefaultLanguage;

                var genericModelBinder = new GenericModelBinder<ExpCategory>();
                var result = genericModelBinder.BindModel(src) as ExpCategory;
                if (!result.Category.Outlines.IsNullOrEmpty())
                {
                    result.Outline = result.Category.Outlines.GetOutlinePath(store.Catalog);
                    result.Slug = result.Category.Outlines.GetSeoPath(store, language.ToString(), null);
                }
                if (result.Outline != null)
                {
                    //Need to take virtual parent from outline (get second last) because for virtual catalog category.ParentId still points to a physical category
                    result.ParentId = result.Outline.Split("/").Reverse().Skip(1).Take(1).FirstOrDefault() ?? result.ParentId;
                }

                if (!result.Category.SeoInfos.IsNullOrEmpty())
                {
                    result.SeoInfo = result.Category.SeoInfos.GetBestMatchingSeoInfo(store.Id, language.ToString()) ?? new SeoInfo
                    {
                        SemanticUrl = result.Id,
                        LanguageCode = language.ToString(),
                        Name = result.Category.Name
                    }; 
                }
                return result;
            });
        }
    }
}
