using VirtoCommerce.ExperienceApiModule.Core.Queries;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class SlugInfoResponseType : ExtendableGraphType<SlugInfoResponse>
    {
        public SlugInfoResponseType()
        {
            Field<SeoInfoType>("entityInfo", "SEO info", resolve: context => context.Source.EntityInfo);
        }
    }
}
