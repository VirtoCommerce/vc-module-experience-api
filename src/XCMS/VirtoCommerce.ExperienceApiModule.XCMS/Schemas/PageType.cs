using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XCMS.Schemas
{
    public class PageType : ObjectGraphType<PageItem>
    {
        public PageType()
        {
            Field(x => x.Name, nullable: true).Description("Page title");
            Field(x => x.RelativeUrl, nullable: true).Description("Page relative url");
        }
    }
}
