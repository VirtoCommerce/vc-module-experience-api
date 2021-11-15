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
            Field(x => x.Name, nullable: true);
            Field(x => x.SeoObjectType, nullable: true);
            Field<ListGraphType<SeoInfoType>>("seoInfos",
                "SEO infos",
                resolve: context => context.Source.SeoInfos);
        }
    }
}
