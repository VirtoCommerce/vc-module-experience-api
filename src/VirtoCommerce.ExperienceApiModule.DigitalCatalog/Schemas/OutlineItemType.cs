using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Outlines;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class OutlineItemType : ObjectGraphType<OutlineItem>
    {
        public OutlineItemType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Name, nullable: false);
            Field(x => x.SeoObjectType, nullable: false);
            Field<ListGraphType<NonNullGraphType<SeoInfoType>>>("seoInfos",
                "SEO info",
                resolve: context => context.Source.SeoInfos);
        }
    }
}
