using GraphQL.Types;
using VirtoCommerce.XCMS.Core.Models;

namespace VirtoCommerce.XCMS.Core.Schemas
{
    public class MenuLinkListType : ObjectGraphType<Menu>
    {
        public MenuLinkListType()
        {
            Field(x => x.Name, nullable: false).Description("Menu name");
            Field(x => x.OuterId, nullable: true).Description("Menu outer ID");
            Field<NonNullGraphType<ListGraphType<NonNullGraphType<MenuLinkType>>>>("items", resolve: context => context.Source.Items);
        }
    }
}
