using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class PageType : ObjectGraphType<PageItem>
    {
        public PageType()
        {
            Field(x => x.Name, nullable: false).Description("Page title");
            Field(x => x.RelativeUrl, nullable: false).Description("Page relative url");
        }
    }
}
