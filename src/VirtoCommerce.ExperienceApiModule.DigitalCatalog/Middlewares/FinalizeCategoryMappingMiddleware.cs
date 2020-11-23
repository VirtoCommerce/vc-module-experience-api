using System;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.XDigitalCatalog.Extensions;
using VirtoCommerce.XDigitalCatalog.Queries;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class FinalizeCategoryMappingMiddleware : IAsyncMiddleware<SearchCategoryResponse>
    {
        public async Task Run(SearchCategoryResponse parameter, Func<SearchCategoryResponse, Task> next)
        {
            var store = parameter.Store;
            var language = parameter.Query.CultureName ?? store.DefaultLanguage;

            foreach (var expCategory in parameter.Results)
            {
                if (!expCategory.Category.Outlines.IsNullOrEmpty())
                {
                    expCategory.Outline = expCategory.Category.Outlines.GetOutlinePath(store.Catalog);
                    expCategory.Slug = expCategory.Category.Outlines.GetSeoPath(store, language.ToString(), null);
                }
                if (expCategory.Outline != null)
                {
                    //Need to take virtual parent from outline (get second last) because for virtual catalog category.ParentId still points to a physical category
                    expCategory.ParentId = expCategory.Outline.Split("/").Reverse().Skip(1).Take(1).FirstOrDefault() ?? expCategory.ParentId;
                }

                if (!expCategory.Category.SeoInfos.IsNullOrEmpty())
                {
                    expCategory.SeoInfo = expCategory.Category.SeoInfos.GetBestMatchingSeoInfo(store.Id, language.ToString()) ?? new SeoInfo
                    {
                        SemanticUrl = expCategory.Id,
                        LanguageCode = language.ToString(),
                        Name = expCategory.Category.Name
                    };
                }
            }

            await next(parameter);
        }
    }
}
