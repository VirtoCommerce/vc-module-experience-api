using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Outlines;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class OutlineItemType : ObjectGraphType<OutlineItem>
    {
        public OutlineItemType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.SeoObjectType);
        }
    }
}
