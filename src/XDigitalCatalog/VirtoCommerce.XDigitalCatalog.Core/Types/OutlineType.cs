using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Outlines;

namespace VirtoCommerce.XDigitalCatalog.Core.Types
{
    public class OutlineType : ObjectGraphType<Outline>
    {
        public OutlineType()
        {
            Field<ListGraphType<NonNullGraphType<OutlineItemType>>>("items",
                "Outline items",
                resolve: context => context.Source.Items);
        }
    }
}
