using System.IO;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.XCMS.Core.Models;

namespace VirtoCommerce.XCMS.Core.Schemas
{
    public class PageType : ObjectGraphType<PageItem>
    {
        private readonly IContentService _contentService;

        public PageType(IContentService contentService)
        {
            _contentService = contentService;

            Field(x => x.Id, nullable: false);
            Field(x => x.Name, nullable: true).Description("Page title");
            Field(x => x.RelativeUrl, nullable: false).Description("Page file relative url");
            Field(x => x.Permalink, nullable: true).Description("Page permalink");
            Field(x => x.Content, nullable: false).ResolveAsync(LoadContent);
        }

        protected virtual async Task<string> LoadContent(IResolveFieldContext<PageItem> context)
        {
            try
            {
                var storeId = context.GetArgumentOrValue<string>("storeId");

                using var stream = await _contentService.GetItemStreamAsync("pages", storeId,
                    context.Source.RelativeUrl);
                using var streamReader = new StreamReader(stream);
                return await streamReader.ReadToEndAsync();
            }
            catch (System.Exception)
            {
                throw new FileNotFoundException(context.Source.RelativeUrl);
            }
        }
    }
}
