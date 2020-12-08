using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class SeoInfoType : ObjectGraphType<SeoInfo>
    {
        public SeoInfoType()
        {
            Name = "SeoInfo";

            Field(x => x.Id, nullable: true);
            Field(x => x.Name, nullable: true);
            Field(x => x.SemanticUrl, nullable: true);
            Field(x => x.PageTitle, nullable: true);
            Field(x => x.MetaDescription, nullable: true);
            Field(x => x.ImageAltDescription, nullable: true);
            Field(x => x.MetaKeywords, nullable: true);
            Field(x => x.StoreId, nullable: true);
            Field(x => x.ObjectId, nullable: true);
            Field(x => x.ObjectType, nullable: true);
            Field(x => x.IsActive, nullable: true);
            Field(x => x.LanguageCode, nullable: true);
        }
    }
}
